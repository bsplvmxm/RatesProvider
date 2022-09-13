using IncredibleBackendContracts.Events;
using Microsoft.Extensions.Logging;
using Moq;
using Polly;
using RatesProvider.Handler.Infrastructure;
using RatesProvider.Handler.Interfaces;
using RatesProvider.RatesGetter.Infrastructure;
using RatesProvider.RatesGetter.Interfaces;
using RatesProvider.Recipient.Interfaces;

namespace RatesProvider.Handler.Tests;

public  class PrimarySourceHandlerTests
{
    private PrimarySourceHandler _sut;
    private Mock<IHandleChecker> _handleChecker;
    private Mock<ILogger> _logger;
    private Mock<ISettingsProvider> _settingsProvider;
    private Mock<IRatesBuilder> _ratesBuilder;
    private Mock<ISyncPolicy> _retryPolicy;

    public void SetUp()
    {
        _logger = new Mock<ILogger>();
        _settingsProvider = new Mock<ISettingsProvider>();
        _ratesBuilder = new Mock<IRatesBuilder>();
        _retryPolicy = new Mock<ISyncPolicy>();
        _handleChecker = new Mock<IHandleChecker>();

        _sut = new(_logger.Object, _settingsProvider.Object, _ratesBuilder.Object, _retryPolicy.Object);
    }

    [Fact]
    public async Task HandleTest_WhenOk_KeepHandleAsync()
    {
        SetUp();

        var handleCheckerTask = Task.FromResult<NewRatesEvent>(new() 
        {
            Rates = 
            { 
                { "USDRUB", (decimal)60 } 
            } 
        });

        _handleChecker
            .Setup(x => x.Check(It.IsAny<IRatesGetter>()))
            .Returns(handleCheckerTask);

        var actual = await _sut.Handle();

        var a = 0;
    }
}
