using IncredibleBackendContracts.Events;
using Microsoft.Extensions.Logging;
using Moq;
using Polly;
using Polly.Retry;
using RatesProvider.Handler.Infrastructure;
using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient.Interfaces;

namespace RatesProvider.Handler.Tests;

public class PrimaryHandleCheckerTests
{
    private PrimaryHandleChecker _sut;
    private RetryPolicySettings _retryPolicySettings;
    private Mock<ILogger> _loggerMock;
    private Mock<ILogger<RetryPolicySettings>> _retryPolicyLoggerMock;
    private Mock<IRatesBuilder> _ratesBuilderMock;
    private Mock<IRatesGetter> _ratesGetterMock;
    private Mock<ISyncPolicy> _pollyMock;

    public void Setup()
    {
        _loggerMock = new Mock<ILogger>();
        _ratesBuilderMock = new Mock<IRatesBuilder>();
        _ratesGetterMock = new Mock<IRatesGetter>();
        _retryPolicyLoggerMock = new Mock<ILogger<RetryPolicySettings>>();
        _pollyMock = new Mock<ISyncPolicy>();

        _retryPolicySettings = new(_retryPolicyLoggerMock.Object);

        _sut = new(_loggerMock.Object, _ratesBuilderMock.Object, _retryPolicySettings.BuildRetryPolicy());
    }

    [Fact]
    public async Task CheckTest_WhenOk_BuildRatesAndReturnOneAsync()
    {
        Setup();

        var expectedResult = CreateExpectedResult();
        var ratesBuilderReturningValue = CreateRatesBuilderReturningValue();

        SetupForPositiveTest(expectedResult, ratesBuilderReturningValue);

        var actual = await _sut.Check(_ratesGetterMock.Object);

        //_pollyMock.Verify(x => x.Execute(It.IsAny<Func<Task<string>>>()), Times.Once); WRYYYYYYYYYYYYYYYYYYYYYYYYYYYY!!!
        _ratesBuilderMock.Verify(x => x.BuildPair<PrimaryRates>(It.IsAny<string>()), Times.Once);
        _ratesGetterMock.Verify(x => x.GetRates(), Times.Once);

        Assert.Equal(expectedResult.Rates.Count, actual.Rates.Count);
        Assert.Equal(expectedResult.Rates.GetType(), actual.Rates.GetType());
    }

    [Fact]
    public async Task CheckTest_WhenIsNotOk_LogExceptionAndReturnEmptyCollectionAsync()
    {
        Setup();

        var expectedResult = CreateExpectedResult();
        var ratesBuilderReturningValue = CreateRatesBuilderReturningValue();

        SetupForNegativeTest(expectedResult, ratesBuilderReturningValue);

        var actual = await _sut.Check(_ratesGetterMock.Object);

        _ratesBuilderMock.Verify(x => x.BuildPair<PrimaryRates>(It.IsAny<string>()), Times.Once);
        _ratesGetterMock.Verify(x => x.GetRates(), Times.Once);

        Assert.Equal(expectedResult.Rates.Count, actual.Rates.Count);
        Assert.Equal(expectedResult.Rates.GetType(), actual.Rates.GetType());
    }

    private void SetupForPositiveTest(NewRatesEvent expectedResult, PrimaryRates ratesBuilderReturningValue)
    {
        var retryPolicyTask = Task.FromResult<string>("qwe");
        var ratesGetterTask = Task.FromResult<string>("qwe");

        _pollyMock
            .Setup(x => x.Execute(It.IsAny<Func<Task<string>>>()))
            .Returns(retryPolicyTask);

        _ratesGetterMock
            .Setup(x => x.GetRates())
            .Returns(ratesGetterTask);

        _ratesBuilderMock
            .Setup(x => x.BuildPair<PrimaryRates>(It.IsAny<string>()))
            .Returns(ratesBuilderReturningValue);
    }

    private void SetupForNegativeTest(NewRatesEvent expectedResult, PrimaryRates ratesBuilderReturningValue)
    {
        Task<string> retryPolicyTask = null!;
        var ratesGetterTask = Task.FromResult<string>("qwe");

        _pollyMock
            .Setup(x => x.Execute(It.IsAny<Func<Task<string>>>()))
            .Returns(retryPolicyTask!);

        _ratesGetterMock
            .Setup(x => x.GetRates())
            .Returns(ratesGetterTask);

        _ratesBuilderMock
            .Setup(x => x.BuildPair<PrimaryRates>(It.IsAny<string>()))
            .Returns(ratesBuilderReturningValue);
    }

    private NewRatesEvent CreateExpectedResult() =>
        new NewRatesEvent()
        {
            Rates =
            {
                { "USDRUB", (decimal)60.09090 },
                { "USDEUR", (decimal)1.09090 }
            }
        };

    private PrimaryRates CreateRatesBuilderReturningValue() =>
        new PrimaryRates()
        {
            Quotes =
            {
                { "USDRUB", (decimal)60.09090 },
                { "USDEUR", (decimal)1.09090 }
            }
        };
}
