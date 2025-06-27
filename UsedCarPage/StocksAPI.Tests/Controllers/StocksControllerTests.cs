using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using StocksAPI.Controllers;
using StocksAPI.BAL.Interfaces;
using StocksAPI.DTOs;

namespace StocksAPI.Tests.Controllers
{
    public class StocksControllerTests
    {
        private readonly Mock<IStockBAL> _mockStockBAL;
        private readonly Mock<ILogger<StocksController>> _mockLogger;
        private readonly StocksController _controller;

        public StocksControllerTests()
        {
            _mockStockBAL = new Mock<IStockBAL>();
            _mockLogger = new Mock<ILogger<StocksController>>();
            _controller = new StocksController(_mockStockBAL.Object, _mockLogger.Object);
        }

        private StockDTO GetSampleStockDTO()
        {
            return new StockDTO
            {
                ProfileId = 1,
                MakeName = "Toyota",
                ModelName = "Corolla",
                MakeYear = 2019,
                Price = 150000,
                FormattedPrice = "₹1.5 Lakh",
                CarName = "Toyota Corolla",
                Km = 8500,
                Fuel = "Petrol",
                CityName = "Mumbai",
                IsValueForMoney = true,
                CreatedDate = DateTime.UtcNow,
                ImageUrl = "https://example.com/car.jpg",
                StockImages = new List<string> { "img1.jpg", "img2.jpg" },
                EmiText = "₹2,500/month",
                TagText = "Certified"
            };
        }

        [Fact]
        public async Task GetStockById_ValidId_ReturnsOkWithStock()
        {
            // Arrange
            var stock = GetSampleStockDTO();
            _mockStockBAL.Setup(s => s.GetStockByIdAsync(stock.ProfileId)).ReturnsAsync(stock);

            // Act
            var result = await _controller.GetStockById(stock.ProfileId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeOfType<StockDTO>();

            var returnedStock = okResult.Value as StockDTO;
            returnedStock!.ProfileId.Should().Be(stock.ProfileId);
            returnedStock.MakeName.Should().Be("Toyota");
            returnedStock.ModelName.Should().Be("Corolla");
            returnedStock.IsValueForMoney.Should().BeTrue();
            returnedStock.StockImages.Should().Contain("img1.jpg");
        }

        [Fact]
        public async Task GetStockById_InvalidId_ReturnsBadRequest()
        {
            // Act
            var result = await _controller.GetStockById(0);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Stock ID must be greater than 0");
        }

        [Fact]
        public async Task GetStockById_NotFound_ReturnsNotFound()
        {
            // Arrange
            int id = 999;
            _mockStockBAL.Setup(s => s.GetStockByIdAsync(id)).ReturnsAsync((StockDTO?)null);

            // Act
            var result = await _controller.GetStockById(id);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>()
                .Which.Value.Should().Be($"Stock with ID {id} not found");
        }

        [Fact]
        public async Task GetStockById_Exception_ReturnsInternalServerError()
        {
            // Arrange
            int id = 1;
            _mockStockBAL.Setup(s => s.GetStockByIdAsync(id)).ThrowsAsync(new Exception("Unexpected"));

            // Act
            var result = await _controller.GetStockById(id);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task SearchStocks_ValidRequest_ReturnsOkWithResults()
        {
            // Arrange
            var request = new StockSearchRequestDTO { PageNumber = 1, PageSize = 10 };
            var stock = GetSampleStockDTO();
            var response = new StockSearchResponseDTO
            {
                Stocks = new List<StockDTO> { stock },
                TotalCount = 1,
                PageNumber = 1,
                PageSize = 10,
                TotalPages = 1,
                HasNextPage = false,
                HasPreviousPage = false
            };

            _mockStockBAL.Setup(s => s.SearchStocksAsync(request)).ReturnsAsync(response);

            // Act
            var result = await _controller.SearchStocks(request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            var resultData = okResult!.Value as StockSearchResponseDTO;

            resultData.Should().NotBeNull();
            resultData!.Stocks.Should().HaveCount(1);
            resultData.Stocks.First().MakeName.Should().Be("Toyota");
        }

        [Fact]
        public async Task SearchStocks_ArgumentException_ReturnsBadRequest()
        {
            // Arrange
            var request = new StockSearchRequestDTO();
            _mockStockBAL.Setup(s => s.SearchStocksAsync(request))
                         .ThrowsAsync(new ArgumentException("Invalid input"));

            // Act
            var result = await _controller.SearchStocks(request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>()
                .Which.Value.Should().Be("Invalid input");
        }

        [Fact]
        public async Task SearchStocks_Exception_ReturnsInternalServerError()
        {
            // Arrange
            var request = new StockSearchRequestDTO();
            _mockStockBAL.Setup(s => s.SearchStocksAsync(request))
                         .ThrowsAsync(new Exception("Unexpected"));

            // Act
            var result = await _controller.SearchStocks(request);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                .Which.StatusCode.Should().Be(500);
        }

        [Fact]
        public void HealthCheck_ReturnsHealthyStatus()
        {
            // Act
            var result = _controller.HealthCheck();

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeAssignableTo<dynamic>();
            var status = okResult.Value!.GetType().GetProperty("status")!.GetValue(okResult.Value, null)!.ToString();
            status.Should().Be("healthy");
        }
    }
}
