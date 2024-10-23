namespace PriceQuery.Tests;
using System.Collections.Generic;
using System.Threading.Tasks;
using PriceQuery.Service.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class PriceUtilityTests
{
    private readonly PriceUtility _priceUtility;
    private readonly Mock<ILogger<PriceUtility>> _mockLogger;

    public PriceUtilityTests()
    {
        _mockLogger = new Mock<ILogger<PriceUtility>>();
        _priceUtility = new PriceUtility(_mockLogger.Object);
    }

    [Fact]
    public async Task GetInstruments_ReturnsListOfInstruments()
    {
        // Act
        var instruments = await _priceUtility.GetInstruments();

        // Assert
        Assert.NotNull(instruments);
        Assert.Contains("BTCUSD", instruments);
        Assert.Contains("ETHUSD", instruments);
        Assert.Contains("EURUSD", instruments);
    }

    [Fact]
    public async Task GetPrice_ReturnsPriceIfPresent()
    {
        // Arrange
        await _priceUtility.UpdatePrice("BTCUSD", 50000);

        // Act
        var price = await _priceUtility.GetPrice("BTCUSD");

        // Assert
        Assert.NotNull(price);
        Assert.Equal(50000, price);
    }

    [Fact]
    public async Task GetPrice_ReturnsNullIfNotPresent()
    {
        // Act
        var price = await _priceUtility.GetPrice("NONEXISTENT");

        // Assert
        Assert.Null(price);
    }

    [Fact]
    public async Task UpdatePrice_ThrowsExceptionIfInstrumentIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _priceUtility.UpdatePrice(null, 50000));
    }

    [Fact]
    public async Task UpdatePrice_ThrowsExceptionIfPriceIsNegative()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _priceUtility.UpdatePrice("BTCUSD", -100));
    }

    [Fact]
    public async Task UpdatePrice_UpdatesPrice()
    {
        // Act
        await _priceUtility.UpdatePrice("BTCUSD", 50000);
        var price = await _priceUtility.GetPrice("BTCUSD");

        // Assert
        Assert.Equal(50000, price);
    }
}
