using System.Text;
using System.Text.Json;
using Micro.Core.Common.Infra.Messaging;
using Micro.Inventory.Products.CreateProduct.Events;

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

    public bool PublishMessage(string routingKey, string type, IMessage message)
    {
        switch (type)
        {
            case "ProductCreated":
                var json = JsonSerializer.Serialize(message as ProductCreatedEvent);
                var byteArray = Encoding.UTF8.GetBytes(json);
                return _messageProducer.PublishMessage(MessagingConstants.DirectExchange, routingKey, byteArray);
            default:
                return _messageProducer.PublishMessage(MessagingConstants.DirectExchange, routingKey, message);
        }
    }
}