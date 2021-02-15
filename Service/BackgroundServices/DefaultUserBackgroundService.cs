using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Glasswall.IdentityManagementService.Common.Models.Store;
using Glasswall.IdentityManagementService.Common.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Glasswall.IdentityManagementService.Api.BackgroundServices
{
    public class DefaultUserBackgroundService : IHostedService
    {
        private readonly ILogger<DefaultUserBackgroundService> _logger;
        private readonly IUserService _userService;

        public DefaultUserBackgroundService(
            ILogger<DefaultUserBackgroundService> logger,
            IUserService userService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(DefaultUserBackgroundService)}: Looking for default user");

            await foreach (var user in _userService.GetAllAsync(stoppingToken))
            {
                if (user.Username != DefaultUser.DefaultUserName) continue;

                _logger.LogInformation($"{nameof(DefaultUserBackgroundService)}: Default user found - id {user.Id}");
                return;
            }

            var newUser = new DefaultUser();

            _logger.LogInformation($"{nameof(DefaultUserBackgroundService)}: Creating default user");

            var operationState = await _userService.CreateAsync(newUser, stoppingToken);

            if (operationState?.Errors?.Any() ?? false)
                throw new Exception(FormatErrors(operationState));

            operationState = await _userService.UpdatePasswordAsync(newUser, DefaultUser.DefaultPassword, stoppingToken);

            if (operationState?.Errors?.Any() ?? false)
                throw new Exception(FormatErrors(operationState));

            _logger.LogInformation($"{nameof(DefaultUserBackgroundService)}: Successfully created default user");
        }

        private static string FormatErrors(UserEditOperationState state)
        {
            return $"{nameof(DefaultUserBackgroundService)}: One or more errors occurred whilst creating default user. " +
                   $"{string.Join(", ", state?.Errors?.SelectMany(s => s.Value?.Select(x => $"{x.ErrorKey}: {x.ErrorMessage}")) ?? Enumerable.Empty<string>())}";
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            return Task.CompletedTask;
        }
    }
}