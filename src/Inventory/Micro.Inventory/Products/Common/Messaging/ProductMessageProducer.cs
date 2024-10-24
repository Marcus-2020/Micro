using Micro.Core.Common.Infra.Messaging;

namespace Micro.Inventory.Products.Common.Messaging;

internal class ProductMessageProducer : IProductMessagingProducer
{
    private readonly IMessageProducer _messageProducer;

    public ProductMessageProducer(IMessageProducer messageProducer)
    {
        _messageProducer = messageProducer;
        
        _messageProducer.DeclareExchange(MessagingConstants.DirectExchange);
        
        _messageProducer.DeclareQueue(
            MessagingConstants.ProductCreated.QueueName,
            MessagingConstants.DirectExchange, 
            MessagingConstants.ProductCreated.RoutingKey);
    }

    public bool PublishMessage(string routingKey, IMessage message)
    {
        return _messageProducer.PublishMessage(MessagingConstants.DirectExchange, routingKey, message);
    }
}