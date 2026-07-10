using System.Text;
using System.Text.Json;
using MediTrack.MedicalAnalysisService.API.Application.Internal.EventHandlers;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Events;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace MediTrack.MedicalAnalysisService.API.Infrastructure.Messaging;

public class HostedEventConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<HostedEventConsumer> _logger;
    private readonly RabbitMqOptions _options;
    private IConnection? _connection;
    private IModel? _channel;

    public HostedEventConsumer(
        IOptions<RabbitMqOptions> options,
        IServiceScopeFactory scopeFactory,
        ILogger<HostedEventConsumer> logger)
    {
        _options = options.Value;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            DispatchConsumersAsync = true
        };

        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            _options.ExchangeName,
            ExchangeType.Topic,
            durable: true);

        _channel.QueueDeclare(
            _options.QueueName,
            durable: true,
            exclusive: false,
            autoDelete: false);

        // Treatment-service publica la creación de recetas con la routing key
        // "PrescriptionCreated" (PrescriptionCreatedEvent), no "PrescriptionLoaded"
        // -- con el binding viejo este consumer nunca recibía nada.
        var routingKeys = new[] { "ComplianceRegistered", "AppointmentAttendanceRegistered", "PrescriptionCreated" };

        foreach (var routingKey in routingKeys)
        {
            _channel.QueueBind(
                _options.QueueName,
                _options.ExchangeName,
                routingKey);
        }

        var consumer = new AsyncEventingBasicConsumer(_channel);

        consumer.Received += async (_, ea) =>
        {
            var routingKey = ea.BasicProperties.Type;

            if (routingKey == null)
            {
                _logger.LogWarning("Received message without Type property. Acknowledging.");
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
                return;
            }

            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                using var scope = _scopeFactory.CreateScope();

                switch (routingKey)
                {
                    case "ComplianceRegistered":
                    {
                        var payload = JsonSerializer.Deserialize<ComplianceRegisteredIntegrationEvent>(json, JsonOptions);
                        if (payload != null)
                        {
                            var handler = scope.ServiceProvider.GetRequiredService<ComplianceRegisteredEventHandler>();
                            await handler.HandleAsync(payload);
                        }
                        break;
                    }
                    case "AppointmentAttendanceRegistered":
                    {
                        var payload = JsonSerializer.Deserialize<AppointmentAttendanceRegisteredIntegrationEvent>(json, JsonOptions);
                        if (payload != null)
                        {
                            var handler = scope.ServiceProvider.GetRequiredService<AppointmentAttendanceRegisteredEventHandler>();
                            await handler.HandleAsync(payload);
                        }
                        break;
                    }
                    case "PrescriptionCreated":
                    {
                        var payload = JsonSerializer.Deserialize<PrescriptionLoadedIntegrationEvent>(json, JsonOptions);
                        if (payload != null)
                        {
                            var handler = scope.ServiceProvider.GetRequiredService<PrescriptionLoadedEventHandler>();
                            await handler.HandleAsync(payload);
                        }
                        break;
                    }
                    default:
                        _logger.LogWarning("Unknown routing key: {RoutingKey}. Acknowledging.", routingKey);
                        _channel.BasicAck(ea.DeliveryTag, multiple: false);
                        return;
                }

                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message with routing key: {RoutingKey}", routingKey);
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
        };

        _channel.BasicConsume(
            _options.QueueName,
            autoAck: false,
            consumer: consumer);

        _logger.LogInformation(
            "Connected to RabbitMQ. Consuming from queue {QueueName} on exchange {ExchangeName}",
            _options.QueueName,
            _options.ExchangeName);

        return Task.CompletedTask;
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping RabbitMQ consumer...");

        _channel?.Close();
        _connection?.Close();

        return base.StopAsync(cancellationToken);
    }

    public override void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
        base.Dispose();
    }
}
