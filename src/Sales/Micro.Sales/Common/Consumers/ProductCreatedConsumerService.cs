using System.Text;
using Micro.Core;
using Micro.Core.Common.Infra.Messaging;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Serilog;

namespace Micro.Sales.Common.Consumers;

public class ProductCreatedConsumerService : IHostedService
{
    private IConnection _connection;
    private IModel _channel;
    private AsyncEventingBasicConsumer _consumer;
    private string? _consumerTag;
    private readonly IServiceProvider _serviceProvider;

    public ProductCreatedConsumerService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
        ConnectionFactory factory = new()
        {
            Uri = new Uri(Configuration.RabbitMqUri),
            ClientProvidedName = Configuration.ServiceName,
            DispatchConsumersAsync = true
        };

        string exchangeName = MessagingConstants.DirectExchange;
        string routingKey = MessagingConstants.ProductCreated.RoutingKey;
        string queueName = MessagingConstants.ProductCreated.QueueName;

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.BasicQos(0, 1, false);

        _channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);
        _channel.QueueDeclare(queueName, false, false, false, null);
        _channel.QueueBind(queueName, exchangeName, routingKey, null);

        _consumer = new AsyncEventingBasicConsumer(_channel);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Log.Logger.Information("Starting ProductCreatedConsumerService");

        _consumer.Received += HandleMessageAsync;

        _consumerTag = _channel.BasicConsume(MessagingConstants.ProductCreated.QueueName, false, _consumer);

        await Task.CompletedTask;
    }

    private async Task HandleMessageAsync(object sender, BasicDeliverEventArgs args)
    {
        var body = args.Body.ToArray();
        string message = Encoding.UTF8.GetString(body);
        Log.Logger
            .ForContext<ProductCreatedConsumerService>()
            .ForContext("message", message)
            .Information("Message received");

        _channel.BasicAck(args.DeliveryTag, false);
        await Task.Yield();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        if (_consumerTag != null)
        {
            _channel.BasicCancel(_consumerTag);
        }
        
        _channel.Close();
        _connection.Close();
        
        return Task.CompletedTask;
    }
}