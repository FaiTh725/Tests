using Application.Shared.Exceptions;
using Authorization.Domain.Entities;
using Authorization.Domain.Interfaces;
using Authorization.Domain.Validators;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Authorization.Infastructure.BackgroundServices
{
    public class InitializeRolesBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;

        public InitializeRolesBackgroundService(
            IServiceScopeFactory scopeFactory)
        {
            this.scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await WaitDatabase(stoppingToken);

            using var scope = scopeFactory.CreateAsyncScope();
            var unitOfWork = scope.ServiceProvider
                .GetRequiredService<IUnitOfWork>();
            var logger = scope.ServiceProvider
                .GetRequiredService<ILogger<InitializeRolesBackgroundService>>();

            var baseRoles = RoleValidator.Roles;
            var existingRoles = await unitOfWork.RoleRepository
                .GetRoles();

            await unitOfWork.BeginTransactionAsync();

            var addRolesTasks = baseRoles.Select(async x =>
            {
                using var innerScope = scopeFactory.CreateAsyncScope();
                var innerUnitOfWork = innerScope.ServiceProvider
                .GetRequiredService<IUnitOfWork>();

                var roleDb = existingRoles
                .FirstOrDefault(role => role.RoleName == x);

                if(roleDb is not null)
                {
                    return;
                }

                var role = Role.Initialize(x);

                if(role.IsFailure)
                {
                    await innerUnitOfWork.RollBackTransactionAsync();
                    logger.LogError("Error initialize role with name " + x);
                    throw new AppConfigurationException("Initialize roles");
                }

                await innerUnitOfWork.RoleRepository.AddRole(role.Value);
                await innerUnitOfWork.SaveChangesAsync();
            }).ToList();

            await Task.WhenAll(addRolesTasks);
            await unitOfWork.CommitTransactionAsync();
            logger.LogInformation("Added the required roles");
        }

        // TODO: Try to find better way to check db healthcheck
        private async Task WaitDatabase(CancellationToken cancellationToken)
        {
            using var scope = scopeFactory.CreateAsyncScope();
            var unitOfWork = scope.ServiceProvider
                .GetRequiredService<IUnitOfWork>();

            while (!cancellationToken.IsCancellationRequested)
            {

                if (await unitOfWork.CanConnectAsync())
                {
                    return;
                }

                await Task.Delay(3000, cancellationToken);
            }
        }
    }
}
