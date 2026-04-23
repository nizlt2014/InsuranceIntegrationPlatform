using System.Text.Json;
using Azure.Messaging.ServiceBus;

namespace PolicyApi.Services;

public class ServiceBusPublisher
{
    private readonly IConfiguration _config;

    public ServiceBusPublisher(IConfiguration config)
    {
        _config = config;
    }

        public async Task PublishAsync<T>(T payload)
    {
        var connection = _config["ServiceBus:ConnectionString"];
        var queueName = _config["ServiceBus:QueueName"];

        await using var client = new ServiceBusClient(connection);
        var sender = client.CreateSender(queueName);

        var json = JsonSerializer.Serialize(payload);

        var message = new ServiceBusMessage(json)
        {
            ContentType = "application/json"
        };

        await sender.SendMessageAsync(message);
    }
}