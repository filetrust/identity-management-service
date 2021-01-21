using System;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Api.ActionFilters;
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

namespace Glasswall.IdentityManagementService.Api
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
            var configuration = ValidateAndBind(Configuration);

            services.TryAddSingleton(configuration);
            services.TryAddSingleton<IFileStoreOptions, UserStoreOptions>();
            services.TryAddScoped<ModelStateValidationActionFilterAttribute>();
            services.TryAddTransient<IFileStore, FileStore>();
            services.TryAddTransient<IUserService, UserService>();
            services.TryAddTransient<ITokenService, JwtTokenService>();
            services.TryAddTransient<IEmailService, EmailService>();
            services.TryAddTransient<IEncryptionHandler, AesEncryptionHandler>();

            services.AddLogging(logging => { logging.AddDebug(); })
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
                            var userId = Guid.Parse(context.Principal.Identity.Name ?? string.Empty);
                            var user = userService.GetByIdAsync(userId, CancellationToken.None).GetAwaiter()
                                .GetResult();
                            if (user == null)
                                // return unauthorized if user no longer exists
                                context.Fail("Unauthorized");
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

            app.Use((context, next) =>
            {
                context.Response.Headers["Access-Control-Allow-Methods"] = "*";
                context.Response.Headers["Access-Control-Expose-Headers"] = "*";
                context.Response.Headers["Access-Control-Allow-Headers"] = "*";
                context.Response.Headers["Access-Control-Allow-Origin"] = "*";

                if (context.Request.Method != "OPTIONS") return next.Invoke();

                context.Response.StatusCode = 200;
                return context.Response.WriteAsync("OK");
            });

            app.UseCors(x => x
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }


        private static IIdentityManagementServiceConfiguration ValidateAndBind(IConfiguration configuration)
        {
            ThrowIfNullOrWhitespace(configuration, nameof(IIdentityManagementServiceConfiguration.TokenLifetime));
            ThrowIfNullOrWhitespace(configuration, nameof(IIdentityManagementServiceConfiguration.TokenSecret));
            ThrowIfNullOrWhitespace(configuration, nameof(IIdentityManagementServiceConfiguration.EncryptionSecret));
            ThrowIfNullOrWhitespace(configuration,
                nameof(IIdentityManagementServiceConfiguration.ManagementUIEndpoint));

            var businessConfig = new IdentityManagementServiceConfiguration();

            configuration.Bind(businessConfig);

            businessConfig.UserStoreRootPath ??= "/mnt/users";

            if (!Directory.Exists(businessConfig.UserStoreRootPath))
                throw new ConfigurationErrorsException(
                    $"A volume must be mounted to '{businessConfig.UserStoreRootPath}'");

            return businessConfig;
        }

        private static void ThrowIfNullOrWhitespace(IConfiguration configuration, string key)
        {
            if (KeyIsNullOrWhiteSpace(configuration, key))
                throw new ConfigurationErrorsException($"{key} was not provided");
        }

        private static bool KeyIsNullOrWhiteSpace(IConfiguration configuration, string key)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            return string.IsNullOrWhiteSpace(configuration[key]);
        }
    }
}