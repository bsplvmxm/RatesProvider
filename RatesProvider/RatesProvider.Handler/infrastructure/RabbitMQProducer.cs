using RatesProvider.Handler.Interfaces;
using RabbitMQ.Client;
using Newtonsoft.Json;
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
            HostName = _settingsProvider.GetEnvironmentVirableValue(EnvironmentVirable.RabbitServer),
            UserName = _settingsProvider.GetEnvironmentVirableValue(EnvironmentVirable.RabbitLogin),
            Password = _settingsProvider.GetEnvironmentVirableValue(EnvironmentVirable.RabbitPassword),
        };
        var connection = factory.CreateConnection();

        using (IModel channel = connection.CreateModel())
        {
            channel.QueueDeclare(Constant.QueueName, true, false, false, null);

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            IBasicProperties basicProperties = channel.CreateBasicProperties();

            channel.BasicPublish(exchange: Constant.Exchange, routingKey: Constant.RootKey, body: body, basicProperties: basicProperties);

            channel.Close();
        }
        connection.Close();
    }
}
