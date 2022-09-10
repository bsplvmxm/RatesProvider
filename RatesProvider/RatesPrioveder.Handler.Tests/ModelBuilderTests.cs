using RatesProvider.Handler;
using RatesProvider.Handler.Interfaces;
using RatesProvider.Handler.Models;
using RatesProvider.Recipient.Exceptions;

namespace RatesPrioveder.Handler.Tests;

public class ModelBuilderTests
{
    private IRatesBuilder _sut;

    public void SetUp()
    {
        _sut = new RatesBuilder();
    }

    [Fact]
    public void BuildPairTest_WhenCorrectJsonFromPrimaryApiRecieved_ShouldReturnPrimaryRateModel()
    {
        SetUp();

        var passedJsonString = "{\"success\": true, \"timestamp\": 1661181484, \"source\": \"USD\", \"quotes\": { \"USDRUB\": 59.874502, \"USDEUR\": 1.004425 }}";

        var expectedModel = new PrimaryRates()
        {
            Quotes = new() {
                { "USDRUB", (decimal)59.874502 },
                { "USDEUR", (decimal)1.004425 }
            }
        };

        var actual = _sut.BuildPair<PrimaryRates>(passedJsonString);

        Assert.Equal(expectedModel.Quotes.Count, actual.Quotes.Count);
        foreach (var pair in expectedModel.Quotes)
        {
            Assert.Equal(pair.Value, actual.Quotes[pair.Key]);
        }
    }

    [Fact]
    public void BuildPairTest_WhenCorrectJsonFromSecondaryApiRecieved_ShouldReturnSecondaryRateModel()
    {
        SetUp();

        var passedJsonString = "{\"success\": true, \"timestamp\": 1661181484, \"source\": \"USD\", \"data\": { \"USDRUB\": \"59.874502\", \"USDEUR\": \"1.004425\" }}";

        var expectedModel = new SecondaryRates()
        {
            Data = new() {
                { "USDRUB", "59.874502" },
                { "USDEUR", "1.004425" }
            }
        };

        var actual = _sut.BuildPair<SecondaryRates>(passedJsonString);

        Assert.Equal(expectedModel.Data.Count, actual.Data.Count);
        foreach (var pair in expectedModel.Data)
        {
            Assert.Equal(pair.Value, actual.Data[pair.Key]);
        }
    }

    [Fact]
    public void BuildPairTest_WhenIncorrectJsonRecieved_ShouldReturnBuildException()
    {
        SetUp();

        var passedJsonString = "{\"success\": true, \"timestamp\": 1661181484, \"source\": \"USD\", \"data\": { \"USDRUB\": \"59.874502\", \"USDEUR\": \"1.004425\" }}";

        Assert.Throws<RatesBuildException>(() => _sut.BuildPair<PrimaryRates>(passedJsonString));
    }

    [Fact]
    public void ConvertToDecimalTest_WhenCalled_ReturnCorrectDictionary()
    {
        SetUp();

        var expectedModel = new PrimaryRates()
        {
            Quotes = new() {
                { "USDRUB", (decimal)59.874502 },
                { "USDEUR", (decimal)1.004425 }
            }
        };

        var ModelToConvert = new SecondaryRates()
        {
            Data = new() {
                { "USDRUB", "59.874502" },
                { "USDEUR", "1.004425" }
            }
        };

        var actualDictionary = _sut.ConvertToDecimal(ModelToConvert.Data);

        foreach (var pair in expectedModel.Quotes)
        {
            Assert.Equal(pair.Value, actualDictionary[pair.Key]);
        }
    }
}