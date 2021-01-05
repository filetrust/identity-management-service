using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Business.Services;
using Glasswall.IdentityManagementService.Business.Store;
using Glasswall.IdentityManagementService.Common.Configuration;
using Glasswall.IdentityManagementService.Common.Services;
using Glasswall.IdentityManagementService.Common.Store;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace Glasswall.PolicyManagement.Api
{
    [ExcludeFromCodeCoverage]
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            if (!Directory.Exists("/mnt/users"))
            {
                throw new ApplicationException("A volume must be mounted to '/mnt/users'");
            }

            var configuration = ValidateAndBind(Configuration);

            services.TryAddSingleton(configuration);
            services.TryAddTransient<IUserService, UserService>();
            services.TryAddTransient<ITokenService, JwtTokenService>();
            services.TryAddTransient<IFileStore>(sp => new MountedFileStore(sp.GetRequiredService<ILogger<MountedFileStore>>(), "/mnt/users"));

            services.AddLogging(logging =>
            {
                logging.AddDebug();
            })
                .Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var userService = context.HttpContext.RequestServices.GetRequiredService<IUserService>();
                        var userId = Guid.Parse(context.Principal.Identity.Name);
                        var user = userService.GetByIdAsync(userId, CancellationToken.None).GetAwaiter().GetResult();
                        if (user == null)
                        {
                            // return unauthorized if user no longer exists
                            context.Fail("Unauthorized");
                        }
                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.TokenSecret)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddControllers();
            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseRouting();
            
            //app.Use((context, next) =>
            //{
            //    context.Response.Headers["Access-Control-Allow-Methods"] = "*";
            //    context.Response.Headers["Access-Control-Expose-Headers"] = "*";
            //    context.Response.Headers["Access-Control-Allow-Headers"] = "*";
            //    context.Response.Headers["Access-Control-Allow-Origin"] = "*";

            //    if (context.Request.Method != "OPTIONS") return next.Invoke();

            //    context.Response.StatusCode = 200;
            //    return context.Response.WriteAsync("OK");
            //});

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }


        private static IIdentityManagementServiceConfiguration ValidateAndBind(IConfiguration configuration)
        {
            ThrowIfNullOrWhitespace(configuration, nameof(IIdentityManagementServiceConfiguration.TokenLifetime));
            ThrowIfNullOrWhitespace(configuration, nameof(IIdentityManagementServiceConfiguration.TokenSecret));
            
            var businessConfig = new IdentityManagementServiceConfiguration();

            configuration.Bind(businessConfig);

            return businessConfig;
        }

        private static void ThrowIfNullOrWhitespace(IConfiguration configuration, string key)
        {
            if (string.IsNullOrWhiteSpace(configuration[key]))
                throw new ConfigurationErrorsException($"{key} was not provided");
        }
    }
}
