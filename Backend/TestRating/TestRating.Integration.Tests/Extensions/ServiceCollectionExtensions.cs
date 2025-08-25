using Microsoft.Extensions.DependencyInjection;

namespace TestRating.Integration.Tests.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void RemoveService(
            this IServiceCollection services, Type type)
        {
            var serviceToRemove = services
                .FirstOrDefault(x => x.ServiceType == type);

            if (serviceToRemove is not null)
            {
                services.Remove(serviceToRemove);
            }
        }

        public static void RemoveServicesByNamespace(
            this IServiceCollection services, string namespaceName)
        {
            var servicesToRemove = services
                    .Where(x => x.ServiceType.Namespace?.StartsWith(namespaceName) == true)
                    .ToList();

            foreach (var serviceToRemove in servicesToRemove)
            {
                services.Remove(serviceToRemove);
            }
        }
    }
}
