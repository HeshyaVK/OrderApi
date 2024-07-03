using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using InventoryService.Models;

public class RabbitMQConsumer
{
    private readonly IConnection _connection;
    private readonly IServiceScopeFactory _scopeFactory;

    public RabbitMQConsumer(IServiceScopeFactory scopeFactory)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = factory.CreateConnection();
        _scopeFactory = scopeFactory;
    }

    public void StartConsuming()
    {
        var channel = _connection.CreateModel();
        channel.QueueDeclare(queue: "order_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var order = JsonSerializer.Deserialize<Order>(message);
            HandleMessage(order);
        };
        channel.BasicConsume(queue: "order_queue", autoAck: true, consumer: consumer);
    }

    private void HandleMessage(Order order)
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<InventoryContext>();
            context.Orders.Add(order);
            context.SaveChanges();
        }
    }
}
