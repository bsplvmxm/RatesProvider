using IncredibleBackendContracts.Events;
using Microsoft.Extensions.Logging;
using Moq;
using Polly;
using RatesProvider.Handler.Infrastructure;
using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.RatesGetter.Interfaces;
using RatesProvider.Recipient.Interfaces;

namespace RatesProvider.Handler.Tests;

public class SecondarySourceHandlerTests
{
    private SecondarySourceHandler _sut;
    private Mock<IHandleChecker> _handleCheckerMock;
    private Mock<ILogger> _loggerMock;
    private Mock<ISettingsProvider> _settingsProviderMock;
    private Mock<IRatesBuilder> _ratesBuilderMock;
    private Mock<ISyncPolicy> _retryPolicyMock;
    private Mock<IRatesGetter> _ratesGetterMock;

    public void SetUp()
    {
        _loggerMock = new Mock<ILogger>();
        _settingsProviderMock = new Mock<ISettingsProvider>();
        _ratesBuilderMock = new Mock<IRatesBuilder>();
        _retryPolicyMock = new Mock<ISyncPolicy>();
        _handleCheckerMock = new Mock<IHandleChecker>();
        _ratesGetterMock = new Mock<IRatesGetter>();

        _sut = new(_loggerMock.Object, _settingsProviderMock.Object, _ratesBuilderMock.Object, _retryPolicyMock.Object);
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

        SetupForPositiveTest(expectedRates, ratesBuilderReturningValue);

        _handleCheckerMock
            .Setup(x => x.Check(It.IsAny<IRatesGetter>()))
            .Returns(handleCheckerTask);

        var actualRates = await _sut.Handle();



        Assert.Equal(expectedRates.Rates.Count, actualRates.Rates.Count);
        Assert.Equal(expectedRates.Rates.GetType(), actualRates.Rates.GetType());
    }

    private void SetupForPositiveTest(NewRatesEvent expectedRates, SecondaryRates ratesBuilderReturningValue)
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
