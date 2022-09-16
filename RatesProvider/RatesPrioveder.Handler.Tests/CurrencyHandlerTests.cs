using IncredibleBackend.Messaging.Interfaces;
using IncredibleBackendContracts.Events;
using Microsoft.Extensions.Logging;
using Moq;
using Polly;
using RatesProvider.Handler.Infrastructure;
using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.RatesGetter.Interfaces;
using RatesProvider.Recipient.Interfaces;
using System.Timers;

namespace RatesProvider.Handler.Tests;

public class CurrencyHandlerTests
{
    private ICurrencyHandler _sut;
    private Mock<IRatesBuilder> _ratesBuilderMock;
    private Mock<ILogger<CurrencyHandler>> _loggerMock;
    private Mock<IMessageProducer> _messageProducerMock;
    private Mock<ISettingsProvider> _settingsProviderMock;
    private Mock<IRetryPolicySettings> _retryPolicySettingsMock;
    private Mock<IRatesSourceHandler> _sourceHandlerMock;
    private Mock<ISyncPolicy> _retryPolicyMock;
    private Mock<IRatesGetter> _ratesGetterMock;
    private Mock<IHandleChecker> _handleCheckerMock;
    private Mock<PrimarySourceHandler> _primarySourceHandlerMock;
    private Mock<SecondarySourceHandler> _secondarySourceHandlerMock;

    public void SetUp()
    {
        _ratesBuilderMock = new Mock<IRatesBuilder>();
        _loggerMock = new Mock<ILogger<CurrencyHandler>>();
        _messageProducerMock = new Mock<IMessageProducer>();
        _settingsProviderMock = new Mock<ISettingsProvider>();
        _retryPolicySettingsMock = new Mock<IRetryPolicySettings>();
        _sourceHandlerMock = new Mock<IRatesSourceHandler>();
        _handleCheckerMock = new Mock<IHandleChecker>();
        _retryPolicyMock = new Mock<ISyncPolicy>();
        _ratesGetterMock = new Mock<IRatesGetter>();
        _primarySourceHandlerMock = new Mock<PrimarySourceHandler>();
        _secondarySourceHandlerMock = new Mock<SecondarySourceHandler>();

        _retryPolicySettingsMock
            .Setup(x => x.BuildRetryPolicy())
            .Returns(_retryPolicyMock.Object);

        _sut = new CurrencyHandler(
            _ratesBuilderMock.Object,
            _loggerMock.Object,
            _messageProducerMock.Object,
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

        var handleCheckerTask = Task.FromResult<NewRatesEvent>(new()
        {
            Rates =
            {
                { "USDRUB", (decimal)60 }
            }
        });

        var expectedRates = new NewRatesEvent()
        {
            Rates =
            {
                { "USDRUB", (decimal)60.09090 },
                { "USDEUR", (decimal)1.09090 }
            }
        };

        var ratesBuilderReturningValue = new SecondaryRates()
        {
            Data =
            {
                { "USDRUB", "60.09090" },
                { "USDEUR", "1.09090" }
            }
        };

        SetUpWhenPrimarySourceHandlerWorksFine(expectedRates, ratesBuilderReturningValue);

        _handleCheckerMock
            .Setup(x => x.Check(It.IsAny<IRatesGetter>()))
            .Returns(handleCheckerTask);

        _primarySourceHandlerMock
            .Setup(x => x.Handle())
            .Returns(sourceHandlerTask);

        _sut.HandleAsync(this, EventArgs.Empty as ElapsedEventArgs);

        _messageProducerMock.Verify
            (x => x.ProduceMessage
                (It.Is<NewRatesEvent>
                    (x => x.Rates.Count == sourceHandlerTask.Result.Rates.Count), It.IsAny<string>()), Times.Once);
    }

    public void SetUpWhenPrimarySourceHandlerWorksFine(NewRatesEvent expectedRates, SecondaryRates ratesBuilderReturningValue )
    {
        var retryPolicyTask = Task.FromResult<string>("qwe");
        var ratesGetterTask = Task.FromResult<string>("qwe");

        _retryPolicyMock
            .Setup(x => x.Execute(It.IsAny<Func<Task<string>>>()))
            .Returns(retryPolicyTask);

        _ratesGetterMock
            .Setup(x => x.GetRates())
            .Returns(ratesGetterTask);

        _ratesBuilderMock
            .Setup(x => x.BuildPair<SecondaryRates>(It.IsAny<string>()))
            .Returns(ratesBuilderReturningValue);

        _ratesBuilderMock
            .Setup(x => x.ConvertToDecimal(It.IsAny<Dictionary<string, string>>()))
            .Returns(expectedRates.Rates);
    }
}
