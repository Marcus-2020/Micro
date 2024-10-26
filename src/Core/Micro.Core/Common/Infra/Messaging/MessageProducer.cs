using System.Text;
using System.Text.Json;
using System.Threading.Channels;
using RabbitMQ.Client;

namespace Micro.Core.Common.Infra.Messaging;

internal class MessageProducer : IMessageProducer
{
    private readonly IConnection _connection;
    private readonly IModel? _channel;
    
    public MessageProducer()
    {
        ConnectionFactory factory = new();
        factory.Uri = new Uri(Configuration.RabbitMqUri);
        factory.ClientProvidedName = Configuration.ServiceName;

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public void DeclareExchange(string exchangeName, string exchangeType = ExchangeType.Direct)
    {
        if (_channel is null) throw new InvalidOperationException("Can't declare a exchange without creating a channel");
        _channel.ExchangeDeclare(exchangeName, exchangeType);
    }

    public void DeclareQueue(string queueName, string exchangeName, string routingKey)
    {
        if (_channel is null) throw new InvalidOperationException("Can't declare a queue without creating a channel");
        _channel.QueueDeclare(queueName, false, false, false, null);
        _channel.QueueBind(queueName, exchangeName, routingKey, null);
    }
    
    public bool PublishMessage(string exchangeName, string routingKey, IMessage message)
    {
        try
        {
            lock (_channel)
            {
                string json = JsonSerializer.Serialize(message);
                byte[] messageBytes = Encoding.UTF8.GetBytes(json);

                _channel.BasicPublish(exchangeName, routingKey, null, messageBytes);
            }
            return true;
        }
        catch (ChannelClosedException)
        {
            return false;
        }
    }

    public bool PublishMessage(string exchangeName, string routingKey, byte[] message,
        Dictionary<string, object>? basicPropertiesHeaders = null)
    {
        try
        {
            if (_channel == null) return false;
            
            lock (_channel)
            {
                IBasicProperties? properties = null;

                if (basicPropertiesHeaders is not null)
                {
                    properties = _channel.CreateBasicProperties();
                    properties.Headers = basicPropertiesHeaders;
                }

                _channel.BasicPublish(exchangeName, routingKey, properties, message);
            }

            return true;
        }
        catch (ChannelClosedException)
        {
            return false;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        if (!disposing) return;
        _channel?.Close();
        _channel?.Dispose();
        _connection?.Close();
        _connection?.Dispose();
    }
}