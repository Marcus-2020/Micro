using Micro.Core.Common.Infra.Messaging;

namespace Micro.Inventory.Products.Common.Messaging;

internal interface IProductMessageProducer
{
    bool PublishMessage(string routingKey, string type, IMessage message);
}