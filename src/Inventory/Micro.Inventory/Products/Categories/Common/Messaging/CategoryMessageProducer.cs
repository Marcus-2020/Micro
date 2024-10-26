using System.Text;
using System.Text.Json;
using Micro.Core.Common.Infra.Messaging;
using Micro.Inventory.Contracts.Products.Categories.Common.Events;
using EventNames = Micro.Inventory.Contracts.Products.Categories.Common.Events.EventNames;

namespace Micro.Inventory.Products.Categories.Common.Messaging;

public class CategoryMessageProducer : ICategoryMessageProducer
{
    private readonly IMessageProducer _messageProducer;

    public CategoryMessageProducer(IMessageProducer messageProducer)
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
            case EventNames.CategoryCreated:
                var categoryCreated = message as CategoryCreatedEvent;
                if (categoryCreated is null) return false;
                
                var json = JsonSerializer.Serialize(categoryCreated);
                var byteArray = Encoding.UTF8.GetBytes(json);
                Dictionary<string, object> props = new()
                {
                    { nameof(CategoryCreatedEvent.Type), categoryCreated.Type },
                    { nameof(CategoryCreatedEvent.Origin), categoryCreated.Origin },
                    { nameof(CategoryCreatedEvent.Version), categoryCreated.Version },
                };

                return _messageProducer.PublishMessage(MessagingConstants.DirectExchange, routingKey, byteArray,
                    basicPropertiesHeaders: props);
            default:
                return _messageProducer.PublishMessage(MessagingConstants.DirectExchange, routingKey, message);
        }
    }
}