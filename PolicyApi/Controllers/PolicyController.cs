using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolicyApi.Models;
using PolicyApi.Services;

namespace PolicyApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PoliciesController : ControllerBase
{
    private readonly ServiceBusPublisher _publisher;

    public PoliciesController(ServiceBusPublisher publisher)
    {
        _publisher = publisher;
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(int policyId)
    {
        var evt = new PolicyCreatedEvent
        {
            PolicyId = policyId,
            CustomerId = 101,
            Plan = "Motor Gold",
            Premium = 15000,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _publisher.PublishAsync(evt);

        return Ok("Policy created and JSON event sent");
    }

    [HttpGet("public")]
    public IActionResult Public()
    {
        return Ok("Public endpoint working");
    }

    [Authorize]
    [HttpGet("secure")]
    public IActionResult Secure()
    {
        return Ok("Secure endpoint working");
    }
}