namespace PolicyApi.Models;

public class PolicyCreatedEvent
{
    public int PolicyId { get; set; }
    public int CustomerId { get; set; }
    public string Plan { get; set; } = string.Empty;
    public decimal Premium { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}