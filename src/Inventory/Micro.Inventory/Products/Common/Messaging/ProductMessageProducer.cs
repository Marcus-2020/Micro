using System.Text;
using System.Text.Json;
using Micro.Core.Common.Infra.Messaging;
using Micro.Inventory.Contracts.Products.CreateProduct.Events;

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
                var productCreated = message as ProductCreatedEvent;
                if (productCreated is null) return false;
                
                var json = JsonSerializer.Serialize(productCreated);
                var byteArray = Encoding.UTF8.GetBytes(json);
                Dictionary<string, object> props = new()
                {
                    { nameof(ProductCreatedEvent.Type), productCreated.Type },
                    { nameof(ProductCreatedEvent.Origin), productCreated.Origin },
                    { nameof(ProductCreatedEvent.Version), productCreated.Version },
                };

                return _messageProducer.PublishMessage(MessagingConstants.DirectExchange, routingKey, byteArray,
                    basicPropertiesHeaders: props);
            default:
                return _messageProducer.PublishMessage(MessagingConstants.DirectExchange, routingKey, message);
        }
    }
}