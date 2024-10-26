using Micro.Core.Common.Infra.Messaging;

namespace Micro.Inventory.Products.Categories.Common.Messaging;

public interface ICategoryMessageProducer
{
    bool PublishMessage(string routingKey, string type, IMessage message);
}