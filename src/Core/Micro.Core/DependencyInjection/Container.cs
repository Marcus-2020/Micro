using Micro.Core.Common.Data;
using Micro.Core.Common.Infra.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace Micro.Core.DependencyInjection;

public static class Container
{
    public static void AddCoreServices(this IServiceCollection sc)
    {
        sc.AddTransient<IDataContextFactory, DataContextFactory>();
        sc.AddSingleton<IMessageProducer, MessageProducer>();
    }
}