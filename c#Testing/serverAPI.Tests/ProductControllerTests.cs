using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using serverAPI.Controllers;
using serverAPI.Models;
using serverAPI.Services;
using System.Collections.Generic;

namespace serverAPI.Tests
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _controller = new ProductController(_mockService.Object);
        }

        [Fact]
        public void GetAll_ShouldReturnAllProducts()
        {
            // Arrange
            var products = new List<Product>
            {
                new Product { Id = 1, Name = "Laptop", Price = 1000 },
                new Product { Id = 2, Name = "Mouse", Price = 25 }
            };
            _mockService.Setup(s => s.GetAll()).Returns(products);

            // Act
            var result = _controller.GetAll();

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);

            var returnedProducts = okResult.Value as IEnumerable<Product>;
            returnedProducts.Should().BeEquivalentTo(products);

            _mockService.Verify(s => s.GetAll(), Times.Once);
        }

        [Fact]
        public void GetById_ShouldReturnProduct_WhenProductExists()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Laptop", Price = 1000 };
            _mockService.Setup(s => s.GetById(1)).Returns(product);

            // Act
            var result = _controller.GetById(1);

            // Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(product);

            _mockService.Verify(s => s.GetById(1), Times.Once);
        }

        [Fact]
        public void GetById_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.GetById(99)).Returns((Product?)null);

            // Act
            var result = _controller.GetById(99);

            // Assert
            var notFound = result.Result as NotFoundResult;
            notFound.Should().NotBeNull();
            notFound!.StatusCode.Should().Be(404);

            _mockService.Verify(s => s.GetById(99), Times.Once);
        }

        [Fact]
        public void Create_ShouldReturnCreatedAtAction_WithNewProduct()
        {
            // Arrange
            var newProduct = new Product { Id = 3, Name = "Keyboard", Price = 50 };
            _mockService.Setup(s => s.Add(newProduct)).Returns(newProduct);

            // Act
            var result = _controller.Create(newProduct);

            // Assert
            var createdResult = result.Result as CreatedAtActionResult;
            createdResult.Should().NotBeNull();
            createdResult!.ActionName.Should().Be("GetById");
            createdResult.RouteValues["id"].Should().Be(newProduct.Id);
            createdResult.Value.Should().BeEquivalentTo(newProduct);

            _mockService.Verify(s => s.Add(newProduct), Times.Once);
        }

        [Fact]
        public void Create_ShouldReturnBadRequest_WhenProductIsNull()
        {
            // Act
            var result = _controller.Create(null!);

            // Assert
            var badRequest = result.Result as BadRequestResult;
            badRequest.Should().NotBeNull();
            badRequest!.StatusCode.Should().Be(400);
        }

        [Fact]
        public void Update_ShouldReturnNoContent_WhenUpdateIsSuccessful()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Updated Laptop", Price = 900 };
            _mockService.Setup(s => s.GetById(product.Id)).Returns(product);

            // Act
            var result = _controller.Update(product.Id, product);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.Update(product.Id, product), Times.Once);
        }

        [Fact]
        public void Update_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            var product = new Product { Id = 99, Name = "Non-existent", Price = 999 };
            _mockService.Setup(s => s.GetById(product.Id)).Returns((Product?)null);

            // Act
            var result = _controller.Update(product.Id, product);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _mockService.Verify(s => s.Update(It.IsAny<int>(), It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public void Delete_ShouldReturnNoContent_WhenSuccessful()
        {
            // Arrange
            var product = new Product { Id = 1, Name = "Laptop", Price = 1000 };
            _mockService.Setup(s => s.GetById(product.Id)).Returns(product);

            // Act
            var result = _controller.Delete(product.Id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _mockService.Verify(s => s.Delete(product.Id), Times.Once);
        }

        [Fact]
        public void Delete_ShouldReturnNotFound_WhenProductDoesNotExist()
        {
            // Arrange
            _mockService.Setup(s => s.GetById(99)).Returns((Product?)null);

            // Act
            var result = _controller.Delete(99);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _mockService.Verify(s => s.Delete(It.IsAny<int>()), Times.Never);
        }
    }
}
