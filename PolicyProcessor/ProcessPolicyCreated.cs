using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace PolicyProcessor;

public class PolicyCreatedEvent
{
    public int PolicyId { get; set; }
    public int CustomerId { get; set; }
    public string Plan { get; set; } = string.Empty;
    public decimal Premium { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class ProcessPolicyCreated
{
    private readonly ILogger _logger;

    public ProcessPolicyCreated(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ProcessPolicyCreated>();
    }

    [Function(nameof(ProcessPolicyCreated))]
    public async Task Run(
        [ServiceBusTrigger("policy-created", Connection = "ServiceBusConnection")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions actions)
    {
        var json = message.Body.ToString();

        var evt = JsonSerializer.Deserialize<PolicyCreatedEvent>(json);

        _logger.LogInformation("PolicyId: {id}", evt?.PolicyId);
        _logger.LogInformation("CustomerId: {cust}", evt?.CustomerId);
        _logger.LogInformation("Plan: {plan}", evt?.Plan);
        _logger.LogInformation("Premium: {premium}", evt?.Premium);

        await actions.CompleteMessageAsync(message);
    }
}