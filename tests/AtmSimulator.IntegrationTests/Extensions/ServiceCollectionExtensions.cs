using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace AtmSimulator.IntegrationTests
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection Remove<T>(this IServiceCollection services)
        {
            var serviceDescriptors = services.Where(descriptor => descriptor.ServiceType == typeof(T));

            foreach (var serviceDescriptor in serviceDescriptors)
            {
                services.Remove(serviceDescriptor);
            }

            return services;
        }
    }
}
