using RatesProvider.Handler.Interfaces;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using RatesProvider.RatesGetter.Infrastructure;

namespace RatesProvider.Handler.Infrastructure;

public class RabbitMQProducer : IRabbitMQProducer
{
    public void SendRatesMessage<T>(T message)
    {
        var factory = new ConnectionFactory
        {
            HostName = Constant.Host
        };
        var connection = factory.CreateConnection();

        using
        var channel = connection.CreateModel();

        channel.QueueDeclare(Constant.QueueName, exclusive: false);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(exchange: Constant.Exchange, routingKey: Constant.RootKey, body: body);
    }
}
