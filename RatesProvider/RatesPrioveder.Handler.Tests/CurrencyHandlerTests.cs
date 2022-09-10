using RatesProvider.Handler.Interfaces;
using Moq;
using Microsoft.Extensions.Logging;
using RatesProvider.RatesGetter.Interfaces;
using System.Timers;
using IncredibleBackendContracts.Events;

namespace RatesProvider.Handler.Tests;

public class CurrencyHandlerTests
{
    private ICurrencyHandler _sut;
    private Mock<IRatesBuilder> _ratesBuilderMock;
    private Mock<ILogger<CurrencyHandler>> _loggerMock;
    private Mock<IRabbitMQProducer> _rabbitProducerMock;
    private Mock<ISettingsProvider> _settingsProviderMock;
    private Mock<IRetryPolicySettings> _retryPolicySettingsMock;
    private Mock<IRatesSourceHandler> _sourceHandlerMock;

    public void SetUp()
    {
        _ratesBuilderMock = new Mock<IRatesBuilder>();
        _loggerMock = new Mock<ILogger<CurrencyHandler>>();
        _rabbitProducerMock = new Mock<IRabbitMQProducer>();
        _settingsProviderMock = new Mock<ISettingsProvider>();
        _retryPolicySettingsMock = new Mock<IRetryPolicySettings>();
        _sourceHandlerMock = new Mock<IRatesSourceHandler>();

        _sut = new CurrencyHandler(
            _ratesBuilderMock.Object,
            _loggerMock.Object,
            _rabbitProducerMock.Object,
            _settingsProviderMock.Object,
            _retryPolicySettingsMock.Object);
    }

    [Fact]
    public void HandleAsync_WhenHandleReturnsCurrency_SendMessageToQueue()
    {
        SetUp();

        Dictionary<string, decimal> rates = new Dictionary<string, decimal>()
        { 
            { "USDRUB", (decimal)60.09090 },
            { "USDEUR", (decimal)1.09090 } 
        };
        var sourceHandlerTask = Task.FromResult(new NewRatesEvent() { Rates = rates});

        _sourceHandlerMock
            .Setup(x => x.Handle())
            .Returns(sourceHandlerTask);

        _sut.HandleAsync(this, EventArgs.Empty as ElapsedEventArgs);

        _sourceHandlerMock.Verify(x => x.Handle(), Times.Once);
        _loggerMock.Verify(x => x.LogInformation(It.IsAny<string>()), Times.Once);
        _rabbitProducerMock.Verify
            (x => x.SendRatesMessage
                (It.Is<NewRatesEvent>
                    (x => x.Rates.Count == sourceHandlerTask.Result.Rates.Count)));
    }

    [Fact]
    public void HandleAsync_WhenHandleReturnsCurrencyTwice_SendMessageToQueue()
    {
        SetUp();

        var sourceHandleRepeats = 2;
        var loggerRepeats = 2;

        Dictionary<string, decimal> rates = new Dictionary<string, decimal>();

        var sourceHandlerTask = Task.FromResult(new NewRatesEvent() { Rates = rates });

        _sourceHandlerMock
            .Setup(x => x.Handle())
            .Returns(sourceHandlerTask);

        _sut.HandleAsync(this, EventArgs.Empty as ElapsedEventArgs);

        _sourceHandlerMock.Verify(x => x.Handle(), Times.Exactly(sourceHandleRepeats));
        _loggerMock.Verify(x => x.LogInformation(It.IsAny<string>()), Times.Exactly(loggerRepeats));
        _rabbitProducerMock.Verify
            (x => x.SendRatesMessage
                (It.Is<NewRatesEvent>
                    (x => x.Rates.Count == sourceHandlerTask.Result.Rates.Count)));
    }
}
