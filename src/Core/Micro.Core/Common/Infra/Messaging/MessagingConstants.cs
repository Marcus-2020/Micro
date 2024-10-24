namespace Micro.Core.Common.Infra.Messaging;

public static class MessagingConstants
{
    public const string DirectExchange = "micro.direct";
    public static readonly (string QueueName, string RoutingKey) ProductCreated = (QueueNames.ProductCreated, "product.created");
}

public static class QueueNames
{
    public const string ProductCreated = "product-created";
}