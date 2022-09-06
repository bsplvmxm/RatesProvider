using RatesProvider.Handler.Interfaces;
using RabbitMQ.Client;
using System.Text.Json;
using System.Text;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;
using RatesProvider.Recipient.Infrastructure;

namespace RatesProvider.Handler.Infrastructure;

public class RabbitMQProducer : IRabbitMQProducer
{
    ISettingsProvider _settingsProvider;

    public RabbitMQProducer(ISettingsProvider settingsPovider)
    {
        _settingsProvider = settingsPovider;
    }

    public void SendRatesMessage<T>(T message)
    {
        var factory = new ConnectionFactory
        {
            AutomaticRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(1),
            HostName = _settingsProvider.GetEnvironmentVirableValue(EnvironmentVirable.RabbitServer),
            UserName = _settingsProvider.GetEnvironmentVirableValue(EnvironmentVirable.RabbitLogin),
            Password = _settingsProvider.GetEnvironmentVirableValue(EnvironmentVirable.RabbitPassword),
            //VirtualHost = "nbtest"
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
