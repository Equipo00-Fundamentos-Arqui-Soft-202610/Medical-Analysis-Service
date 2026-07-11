using System.Text;
using System.Text.Json;
using MediTrack.MedicalAnalysisService.API.Application.Internal.EventHandlers;
using MediTrack.MedicalAnalysisService.API.Domain.Model.Events;
using MediTrack.MedicalAnalysisService.API.Infrastructure.Persistence.EFC;
using MediTrack.MedicalAnalysisService.API.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;
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

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // La conexión a RabbitMQ ya no se crea "a pelo": si CloudAMQP está caído
        // justo al arrancar, antes esto tumbaba TODO el proceso (incluyendo los
        // endpoints REST del dashboard), no solo este consumidor. Ahora reintenta
        // con backoff fijo hasta conectar, igual que el resto de los servicios.
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Connect();
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "No se pudo conectar a RabbitMQ; se reintenta en 5 segundos.");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }
    }

    private void Connect()
    {
        var factory = new ConnectionFactory
        {
            HostName = _options.Host,
            Port = _options.Port,
            UserName = _options.UserName,
            Password = _options.Password,
            VirtualHost = _options.VirtualHost,
            DispatchConsumersAsync = true,
            AutomaticRecoveryEnabled = true,
            RequestedHeartbeat = TimeSpan.FromSeconds(30),
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10)
        };

        _connection = factory.CreateConnection("meditrack-analysis-service");
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            _options.ExchangeName,
            ExchangeType.Topic,
            durable: true);

        var dlxName = $"{_options.ExchangeName}.dlx";
        var dlqName = $"{_options.QueueName}.dlq";
        _channel.ExchangeDeclare(dlxName, ExchangeType.Fanout, durable: true);
        _channel.QueueDeclare(dlqName, durable: true, exclusive: false, autoDelete: false);
        _channel.QueueBind(dlqName, dlxName, routingKey: "");

        var mainQueueArgs = new Dictionary<string, object> { { "x-dead-letter-exchange", dlxName } };
        try
        {
            _channel.QueueDeclare(_options.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: mainQueueArgs);
        }
        catch (RabbitMQ.Client.Exceptions.OperationInterruptedException)
        {
            _logger.LogWarning("La cola {QueueName} existía con argumentos distintos; se recrea con soporte de DLQ.", _options.QueueName);
            _channel = _connection!.CreateModel();
            _channel.ExchangeDeclare(_options.ExchangeName, ExchangeType.Topic, durable: true);
            _channel.QueueDelete(_options.QueueName);
            _channel.QueueDeclare(_options.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: mainQueueArgs);
        }

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
            // El routing key AMQP real (no BasicProperties.Type, que es un campo que
            // cada publisher setea "a mano" -- funcionaba antes solo porque los 4
            // publishers con patrón Outbox lo dejan igual al routing key por
            // convención implícita, no documentada).
            var routingKey = ea.RoutingKey;

            try
            {
                var body = ea.Body.ToArray();
                var json = Encoding.UTF8.GetString(body);

                using var scope = _scopeFactory.CreateScope();

                var hasMessageId = Guid.TryParse(ea.BasicProperties.MessageId, out var messageId);
                if (!hasMessageId)
                {
                    _logger.LogWarning(
                        "Mensaje con routing key {RoutingKey} sin MessageId válido; se omite la verificación de duplicados.",
                        routingKey);
                }

                MedicalAnalysisDbContext? context = null;
                if (hasMessageId)
                {
                    context = scope.ServiceProvider.GetRequiredService<MedicalAnalysisDbContext>();

                    var alreadyProcessed = await context.ProcessedEvents.AnyAsync(e => e.EventId == messageId);
                    if (alreadyProcessed)
                    {
                        _logger.LogInformation("Mensaje {MessageId} ya procesado; se omite.", messageId);
                        _channel.BasicAck(ea.DeliveryTag, multiple: false);
                        return;
                    }
                }

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

                if (hasMessageId && context != null)
                {
                    context.ProcessedEvents.Add(new ProcessedEvent
                    {
                        EventId = messageId,
                        EventType = routingKey,
                        ProcessedAtUtc = DateTime.UtcNow
                    });
                    await context.SaveChangesAsync();
                }

                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializando el mensaje con routing key: {RoutingKey}. Se descarta.", routingKey);
                _channel.BasicAck(ea.DeliveryTag, multiple: false);
            }
            catch (Exception ex)
            {
                if (ea.Redelivered)
                {
                    _logger.LogError(ex,
                        "Error persistente procesando routing key {RoutingKey} tras reintento; se envía a DLQ.",
                        routingKey);
                    _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: false);
                }
                else
                {
                    _logger.LogWarning(ex,
                        "Error procesando routing key {RoutingKey}; se reintenta una vez.",
                        routingKey);
                    _channel.BasicNack(ea.DeliveryTag, multiple: false, requeue: true);
                }
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
