using Moq;
using FluentAssertions;
using AutoMapper;
using Grpc.Core;
using StocksAPI.BAL.Services;
using StocksAPI.DAL.Interfaces;
using StocksAPI.DTOs;
using StocksAPI.Entities;
using FinanceService.Protos;


namespace StocksAPI.Tests.BAL
{
    public class StockBALTests
    {
        private readonly Mock<IStockDAL> _mockStockDAL;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<Finance.FinanceClient> _mockGrpcClient;
        private readonly StockBAL _stockBAL;

        public StockBALTests()
        {
            _mockStockDAL = new Mock<IStockDAL>();
            _mockMapper = new Mock<IMapper>();
            _mockGrpcClient = new Mock<Finance.FinanceClient>();
            _stockBAL = new StockBAL(_mockStockDAL.Object, _mockMapper.Object, _mockGrpcClient.Object);
        }

        private Stock GetSampleStockEntity() => new()
        {
            Id = 1,
            MakeName = "Toyota",
            ModelName = "Corolla",
            MakeYear = 2020,
            Price = 150000,
            Kilometers = 9000,
            CityName = "Mumbai",
            CreatedDate = DateTime.UtcNow
        };

        private StockDTO GetSampleStockDTO() => new()
        {
            ProfileId = 1,
            MakeName = "Toyota",
            ModelName = "Corolla",
            MakeYear = 2020,
            Price = 150000,
            Km = 9000,
            CityName = "Mumbai",
            IsValueForMoney = true,
            StockImages = new List<string> { "img1.jpg" },
            CreatedDate = DateTime.UtcNow
        };

        private AsyncUnaryCall<T> CreateAsyncUnaryCall<T>(T response) where T : class
        {
            return new AsyncUnaryCall<T>(
                Task.FromResult(response),
                Task.FromResult(new Metadata()),
                () => Status.DefaultSuccess,
                () => new Metadata(),
                () => { });
        }

        [Fact]
        public async Task SearchStocksAsync_ShouldReturnMappedResult()
        {
            // Arrange
            var request = new StockSearchRequestDTO { PageNumber = 1, PageSize = 10 };
            var filters = new Filters();
            var stockEntities = new List<Stock> { GetSampleStockEntity() };
            var stockDTOs = new List<StockDTO> { GetSampleStockDTO() };

            _mockMapper.Setup(m => m.Map<Filters>(request)).Returns(filters);
            _mockStockDAL.Setup(d => d.GetStocksAsync(filters, 1, 10)).ReturnsAsync(stockEntities);
            _mockStockDAL.Setup(d => d.GetStocksCountAsync(filters)).ReturnsAsync(1);
            _mockMapper.Setup(m => m.Map<List<StockDTO>>(stockEntities)).Returns(stockDTOs);

            var grpcResponse = new ValueForMoneyResponse
            {
                CarStatuses = { new CarStatus { Id = 1, IsValueForMoney = true } }
            };
            _mockGrpcClient.Setup(c => c.GetIsValueForMoneyAsync(It.IsAny<ValueForMoneyRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(CreateAsyncUnaryCall(grpcResponse));

            // Act
            var result = await _stockBAL.SearchStocksAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Stocks.Should().HaveCount(1);
            result.Stocks[0].ProfileId.Should().Be(1);
            result.Stocks[0].IsValueForMoney.Should().BeTrue();
            result.TotalCount.Should().Be(1);
            result.PageNumber.Should().Be(1);
            result.PageSize.Should().Be(10);
            result.TotalPages.Should().Be(1);
        }

        [Fact]
        public async Task GetStockByIdAsync_ShouldReturnMappedDTO()
        {
            // Arrange
            var stockEntity = GetSampleStockEntity();
            var stockDTO = GetSampleStockDTO();

            _mockStockDAL.Setup(d => d.GetStockByIdAsync(stockEntity.Id)).ReturnsAsync(stockEntity);
            _mockMapper.Setup(m => m.Map<StockDTO>(stockEntity)).Returns(stockDTO);

            var grpcResponse = new ValueForMoneyResponse
            {
                CarStatuses = { new CarStatus { Id = 1, IsValueForMoney = true } }
            };
            _mockGrpcClient.Setup(c => c.GetIsValueForMoneyAsync(It.IsAny<ValueForMoneyRequest>(), null, null, It.IsAny<CancellationToken>()))
                .Returns(CreateAsyncUnaryCall(grpcResponse));

            // Act
            var result = await _stockBAL.GetStockByIdAsync(stockEntity.Id);

            // Assert
            result.Should().NotBeNull();
            result!.MakeName.Should().Be("Toyota");
            result.IsValueForMoney.Should().BeTrue();
        }

        [Fact]
        public async Task GetStockByIdAsync_WhenNotFound_ReturnsNull()
        {
            // Arrange
            _mockStockDAL.Setup(d => d.GetStockByIdAsync(999)).ReturnsAsync((Stock?)null);

            // Act
            var result = await _stockBAL.GetStockByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task SearchStocksAsync_ThrowsException_ReturnsWrappedError()
        {
            // Arrange
            var request = new StockSearchRequestDTO();
            _mockMapper.Setup(m => m.Map<Filters>(request)).Throws<Exception>();

            // Act
            Func<Task> act = async () => await _stockBAL.SearchStocksAsync(request);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Error occurred while searching stocks*");
        }

        [Fact]
        public async Task GetStockByIdAsync_ThrowsException_ReturnsWrappedError()
        {
            // Arrange
            _mockStockDAL.Setup(d => d.GetStockByIdAsync(It.IsAny<int>()))
                         .Throws<Exception>();

            // Act
            Func<Task> act = async () => await _stockBAL.GetStockByIdAsync(1);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                     .WithMessage("Error occurred while fetching stock with ID*");
        }
    }
}
