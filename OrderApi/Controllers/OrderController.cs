using Microsoft.AspNetCore.Mvc;
using OrderApi.Models;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly RabbitMQPublisher _rabbitMQPublisher;

    public OrderController(RabbitMQPublisher rabbitMQPublisher)
    {
        _rabbitMQPublisher = rabbitMQPublisher;
    }

    [HttpPost]
    public IActionResult CreateOrder([FromBody] Order order)
    {
        // Publish event to RabbitMQ
        _rabbitMQPublisher.Publish("order_queue", order);

        return Ok(order);
    }
}


