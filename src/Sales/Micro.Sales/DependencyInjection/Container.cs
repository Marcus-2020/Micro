using Micro.Sales.Common.Consumers;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Sales.DependencyInjection;

public static class Container
{
    public static void AddSalesServices(this IServiceCollection sc)
    {
        sc.AddHostedService<ProductCreatedConsumerService>();
    }
}