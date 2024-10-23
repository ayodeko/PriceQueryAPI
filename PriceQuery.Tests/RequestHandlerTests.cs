using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PriceQuery.Core;
using PriceQuery.Core.Interfaces;
using PriceQuery.Service.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class RequestHandlerTests
{
    private readonly Mock<IPriceChannel> _mockPriceChannel;
    private readonly Mock<IPriceUtility> _mockPriceUtility;
    private readonly Mock<ILogger<RequestHandler>> _mockLogger;
    private readonly RequestHandler _requestHandler;

    public RequestHandlerTests()
    {
        _mockPriceChannel = new Mock<IPriceChannel>();
        _mockPriceUtility = new Mock<IPriceUtility>();
        _mockLogger = new Mock<ILogger<RequestHandler>>();
        _requestHandler = new RequestHandler(_mockPriceChannel.Object, _mockPriceUtility.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetInstrumentsRest_ReturnsSuccessfulResponse()
    {
        // Arrange
        var instruments = new List<string> { "BTCUSD", "ETHUSD", "EURUSD" };
        _mockPriceUtility.Setup(p => p.GetInstruments()).ReturnsAsync(instruments);

        // Act
        var response = await _requestHandler.GetInstrumentsRest();

        // Assert
        Assert.NotNull(response);
        Assert.Equal(ResponseCode.Successful, response.responseCode);
        Assert.Equal("Retrieved", response.responseDescription);
        Assert.Equal(instruments, response.result);
    }

    [Fact]
    public async Task GetInstrumentsRest_ReturnsFailedResponseOnException()
    {
        // Arrange
        _mockPriceUtility.Setup(p => p.GetInstruments()).ThrowsAsync(new Exception("Test exception"));

        // Act
        var response = await _requestHandler.GetInstrumentsRest();

        // Assert
        Assert.NotNull(response);
        Assert.Equal(ResponseCode.Failed, response.responseCode);
        Assert.Equal("An error occurred while retrieving the instruments", response.responseDescription);
        Assert.Null(response.result);
    }

    [Fact]
    public async Task GetPriceRest_ReturnsSuccessfulResponseIfPriceExists()
    {
        // Arrange
        var instrument = "BTCUSD";
        var price = 50000m;
        _mockPriceUtility.Setup(p => p.GetPrice(instrument)).ReturnsAsync(price);

        // Act
        var response = await _requestHandler.GetPriceRest(instrument);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(ResponseCode.Successful, response.responseCode);
        Assert.Equal("Retrieved", response.responseDescription);
    }

    [Fact]
    public async Task GetPriceRest_ReturnsFailedResponseIfPriceDoesNotExist()
    {
        // Arrange
        var instrument = "BTCUSD";
        _mockPriceUtility.Setup(p => p.GetPrice(instrument)).ReturnsAsync((decimal?)null);

        // Act
        var response = await _requestHandler.GetPriceRest(instrument);

        // Assert
        Assert.NotNull(response);
        Assert.Equal(ResponseCode.Failed, response.responseCode);
        Assert.Equal($"Price for {instrument} not found, please try again later", response.responseDescription);
        Assert.Null(response.result);
    }

    [Fact]
    public async Task GetPriceRest_ReturnsFailedResponseOnException()
    {
        // Arrange
        var instrument = "BTCUSD";
        _mockPriceUtility.Setup(p => p.GetPrice(instrument)).ThrowsAsync(new Exception("Test exception"));

        // Act
        await Assert.ThrowsAsync<Exception>(() => _requestHandler.GetPriceRest(instrument));
    }
}
