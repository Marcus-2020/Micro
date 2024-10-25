using RabbitMQ.Client;

namespace Micro.Core.Common.Infra.Messaging;

public interface IMessageProducer : IDisposable
{
    void DeclareExchange(string exchangeName, string exchangeType = ExchangeType.Direct);
    void DeclareQueue(string queueName, string exchangeName, string routingKey);
    bool PublishMessage(string exchangeName, string routingKey, IMessage message, bool restartChannelIfFailed = false);
    bool PublishMessage(string exchangeName, string routingKey, byte[] message, bool restartChannelIfFailed = false);
}