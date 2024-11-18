using Micro.Core.Common.Infra.Messaging;

namespace Micro.Inventory.Products.Common.Messaging;

public interface IProductMessageProducer
{
    bool PublishMessage(string routingKey, string type, IMessage message);
}