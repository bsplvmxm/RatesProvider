using IncredibleBackendContracts.Events;
using Microsoft.Extensions.Logging;
using Moq;
using Polly.Retry;
using RatesProvider.Handler.Infrastructure;
using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient.Interfaces;

namespace RatesProvider.Handler.Tests;

public class SecondaryHandleCheckerTests
{
    private SecondaryHandleChecker _sut;
    private RetryPolicySettings _retryPolicySettings;
    private Mock<ILogger> _loggerMock;
    private Mock<ILogger<RetryPolicySettings>> _retryPolicyLoggerMock;
    private Mock<IRatesBuilder> _ratesBuilderMock;
    private Mock<IRatesGetter> _ratesGetterMock;

    public void Setup()
    {
        _loggerMock = new Mock<ILogger>();
        _ratesBuilderMock = new Mock<IRatesBuilder>();
        _ratesGetterMock = new Mock<IRatesGetter>();
        _retryPolicyLoggerMock = new Mock<ILogger<RetryPolicySettings>>();

        _retryPolicySettings = new(_retryPolicyLoggerMock.Object);

        _sut = new(_loggerMock.Object, _ratesBuilderMock.Object, _retryPolicySettings.BuildRetryPolicy());
    }

    [Fact]
    public async Task CheckTest_WhenOk_BuildRatesAndReturnOneAsync()
    {
        Setup();

        var expectedResult = new NewRatesEvent() 
        { Rates = 
            { 
                { "USDRUB", (decimal)60.09090 },
                { "USDEUR", (decimal)1.09090 } 
            } 
        };

        _ratesBuilderMock
            .Setup(x => x.ConvertToDecimal(It.IsAny<Dictionary<string, string>>()))
            .Returns(expectedResult.Rates);

        _ratesBuilderMock.Setup(x => x.BuildPair<SecondaryRates>(It.IsAny<string>()));

        var actual = await _sut.Check(_ratesGetterMock.Object);

        _ratesBuilderMock.Verify(x => x.BuildPair<SecondaryRates>(It.IsAny<string>()), Times.Once);
        _ratesBuilderMock.Verify(x => x.ConvertToDecimal(It.IsAny<Dictionary<string, string>>()), Times.Once);

        Assert.Equal(expectedResult.Rates.Count, actual.Rates.Count);
        Assert.Equal(expectedResult.Rates.GetType(), actual.Rates.GetType());
    }

    [Fact]
    public void CheckTest_WhenIsNotOk_LogExceptionAndReturnEmptyCollection()
    {
        Setup();
    }
}
