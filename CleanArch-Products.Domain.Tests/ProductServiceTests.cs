using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using AutoMapper;
using CleanArch_Products.Application.DTOs;
using CleanArch_Products.Application.Interfaces;
using CleanArch_Products.Application.Services;
using CleanArch_Products.Application.Messaging;
using CleanArch_Products.Domain.Entities;
using MediatR;
using Xunit;

namespace CleanArch_Products.Domain.Tests
{
    public class ProductServiceTests
    {
        private Mock<IMapper> GetMockMapper()
        {
            return new Mock<IMapper>();
        }

        private Mock<IMediator> GetMockMediator()
        {
            return new Mock<IMediator>();
        }

        private Mock<IMessageBus> GetMockMessageBus()
        {
            return new Mock<IMessageBus>();
        }

        #region Add Tests

        [Fact(DisplayName = "Add - Should add product successfully with valid DTO")]
        public async Task Add_WithValidCreateProductDTO_ShouldAddProductSuccessfully()
        {
            // Arrange
            var mockMapper = GetMockMapper();
            var mockMediator = GetMockMediator();
            var mockMessageBus = GetMockMessageBus();

            var service = new ProductService(mockMapper.Object, mockMediator.Object, mockMessageBus.Object);

            var createProductDTO = new CreateProductDTO
            {
                Name = "New Product",
                Description = "Product Description",
                Price = 99.99m,
                Stock = 10,
                Image = "product.jpg",
                CategoryId = 1
            };

            var product = new Product(1, "New Product", "Product Description", 99.99m, 10, "product.jpg", 1);

            mockMediator.Setup(m => m.Send(It.IsAny<IRequest<Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            mockMessageBus.Setup(m => m.PublishAsync(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            // Act
            await service.Add(createProductDTO);

            // Assert
            mockMediator.Verify(m => m.Send(It.IsAny<IRequest<Product>>(), It.IsAny<CancellationToken>()), Times.Once);
            mockMessageBus.Verify(m => m.PublishAsync("product-created", It.IsAny<CreateProductDTO>()), Times.Once);
        }

        [Fact(DisplayName = "Add - Should map CreateProductDTO to ProductCreateCommand")]
        public async Task Add_ShouldMapDTOToCommand()
        {
            // Arrange
            var mockMapper = GetMockMapper();
            var mockMediator = GetMockMediator();
            var mockMessageBus = GetMockMessageBus();

            var service = new ProductService(mockMapper.Object, mockMediator.Object, mockMessageBus.Object);

            var createProductDTO = new CreateProductDTO
            {
                Name = "Laptop",
                Description = "Gaming Laptop",
                Price = 1500.00m,
                Stock = 5,
                Image = "laptop.jpg",
                CategoryId = 2
            };

            mockMessageBus.Setup(m => m.PublishAsync(It.IsAny<string>(), It.IsAny<object>()))
                .Returns(Task.CompletedTask);

            // Act
            await service.Add(createProductDTO);

            // Assert
            mockMapper.Verify(m => m.Map<dynamic>(It.Is<CreateProductDTO>(d => d.Name == "Laptop")), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact(DisplayName = "GetById - Should return product DTO when product exists")]
        public async Task GetById_WithValidId_ShouldReturnProductDTO()
        {
            // Arrange
            var mockMapper = GetMockMapper();
            var mockMediator = GetMockMediator();
            var mockMessageBus = GetMockMessageBus();

            var service = new ProductService(mockMapper.Object, mockMediator.Object, mockMessageBus.Object);

            var product = new Product(1, "Test Product", "Test Description", 100m, 5, "test.jpg", 1);
            var productDTO = new ProductDTO
            {
                Id = 1,
                Name = "Test Product",
                Description = "Test Description",
                Price = 100m,
                Stock = 5
            };

            mockMediator.Setup(m => m.Send(It.IsAny<IRequest<Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            mockMapper.Setup(m => m.Map<ProductDTO>(It.IsAny<Product>()))
                .Returns(productDTO);

            // Act
            var result = await service.GetById(1);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Test Product");
            result.Price.Should().Be(100m);
        }

        [Fact(DisplayName = "GetById - Should map product to ProductDTO")]
        public async Task GetById_ShouldMapProductToDTO()
        {
            // Arrange
            var mockMapper = GetMockMapper();
            var mockMediator = GetMockMediator();
            var mockMessageBus = GetMockMessageBus();

            var service = new ProductService(mockMapper.Object, mockMediator.Object, mockMessageBus.Object);

            var product = new Product(1, "Product", "Description", 99.99m, 10, "image.jpg", 1);
            var productDTO = new ProductDTO { Id = 1, Name = "Product" };

            mockMediator.Setup(m => m.Send(It.IsAny<IRequest<Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            mockMapper.Setup(m => m.Map<ProductDTO>(product))
                .Returns(productDTO);

            // Act
            var result = await service.GetById(1);

            // Assert
            mockMapper.Verify(m => m.Map<ProductDTO>(It.IsAny<Product>()), Times.Once);
            result.Should().Be(productDTO);
        }

        #endregion

        #region GetProductAndCategory Tests

        [Fact(DisplayName = "GetProductAndCategory - Should return product with category details")]
        public async Task GetProductAndCategory_WithValidId_ShouldReturnProductWithCategory()
        {
            // Arrange
            var mockMapper = GetMockMapper();
            var mockMediator = GetMockMediator();
            var mockMessageBus = GetMockMessageBus();

            var service = new ProductService(mockMapper.Object, mockMediator.Object, mockMessageBus.Object);

            var product = new Product(1, "Phone", "Smartphone", 699m, 15, "phone.jpg", 1);
            var productDTO = new ProductDTO
            {
                Id = 1,
                Name = "Phone",
                Description = "Smartphone"
            };

            mockMediator.Setup(m => m.Send(It.IsAny<IRequest<Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            mockMapper.Setup(m => m.Map<ProductDTO>(It.IsAny<Product>()))
                .Returns(productDTO);

            // Act
            var result = await service.GetProductAndCategory(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Phone");
        }

        #endregion

        #region GetProducts Tests

        [Fact(DisplayName = "GetProducts - Should return all products")]
        public async Task GetProducts_ShouldReturnAllProducts()
        {
            // Arrange
            var mockMapper = GetMockMapper();
            var mockMediator = GetMockMediator();
            var mockMessageBus = GetMockMessageBus();

            var service = new ProductService(mockMapper.Object, mockMediator.Object, mockMessageBus.Object);

            var products = new List<Product>
            {
                new Product(1, "Product 1", "Description 1", 100m, 5, "prod1.jpg", 1),
                new Product(2, "Product 2", "Description 2", 200m, 10, "prod2.jpg", 1)
            };

            var productDTOs = new List<ProductDTO>
            {
                new ProductDTO { Id = 1, Name = "Product 1" },
                new ProductDTO { Id = 2, Name = "Product 2" }
            };

            mockMediator.Setup(m => m.Send(It.IsAny<IRequest<IEnumerable<Product>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            mockMapper.Setup(m => m.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()))
                .Returns(productDTOs);

            // Act
            var result = await service.GetProducts();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(p => p.Name == "Product 1");
            result.Should().Contain(p => p.Name == "Product 2");
        }

        [Fact(DisplayName = "GetProducts - Should return empty list when no products exist")]
        public async Task GetProducts_WithNoProducts_ShouldReturnEmptyList()
        {
            // Arrange
            var mockMapper = GetMockMapper();
            var mockMediator = GetMockMediator();
            var mockMessageBus = GetMockMessageBus();

            var service = new ProductService(mockMapper.Object, mockMediator.Object, mockMessageBus.Object);

            mockMediator.Setup(m => m.Send(It.IsAny<IRequest<IEnumerable<Product>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<Product>());

            mockMapper.Setup(m => m.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()))
                .Returns(new List<ProductDTO>());

            // Act
            var result = await service.GetProducts();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        #endregion

        #region GetProductsByCategoryId Tests

        [Fact(DisplayName = "GetProductsByCategoryId - Should return products for specific category")]
        public async Task GetProductsByCategoryId_WithValidCategoryId_ShouldReturnProductsInCategory()
        {
            // Arrange
            var mockMapper = GetMockMapper();
            var mockMediator = GetMockMediator();
            var mockMessageBus = GetMockMessageBus();

            var service = new ProductService(mockMapper.Object, mockMediator.Object, mockMessageBus.Object);

            var products = new List<Product>
            {
                new Product(1, "Product 1", "Description 1", 100m, 5, "prod1.jpg", 1),
                new Product(2, "Product 2", "Description 2", 200m, 10, "prod2.jpg", 1)
            };

            var productDTOs = new List<ProductDTO>
            {
                new ProductDTO { Id = 1, Name = "Product 1" },
                new ProductDTO { Id = 2, Name = "Product 2" }
            };

            mockMediator.Setup(m => m.Send(It.IsAny<IRequest<IEnumerable<Product>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(products);

            mockMapper.Setup(m => m.Map<IEnumerable<ProductDTO>>(It.IsAny<IEnumerable<Product>>()))
                .Returns(productDTOs);

            // Act
            var result = await service.GetProductsByCategoryId(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
        }

        #endregion

        #region Remove Tests

        [Fact(DisplayName = "Remove - Should remove product successfully")]
        public async Task Remove_WithValidId_ShouldRemoveProductSuccessfully()
        {
            // Arrange
            var mockMapper = GetMockMapper();
            var mockMediator = GetMockMediator();
            var mockMessageBus = GetMockMessageBus();

            var service = new ProductService(mockMapper.Object, mockMediator.Object, mockMessageBus.Object);

            var product = new Product(1, "Product to Remove", "Description", 50m, 2, "remove.jpg", 1);

            mockMediator.Setup(m => m.Send(It.IsAny<IRequest<Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            await service.Remove(1);

            // Assert
            mockMediator.Verify(m => m.Send(It.IsAny<IRequest<Product>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion

        #region Update Tests

        [Fact(DisplayName = "Update - Should update product successfully")]
        public async Task Update_WithValidProductDTO_ShouldUpdateProductSuccessfully()
        {
            // Arrange
            var mockMapper = GetMockMapper();
            var mockMediator = GetMockMediator();
            var mockMessageBus = GetMockMessageBus();

            var service = new ProductService(mockMapper.Object, mockMediator.Object, mockMessageBus.Object);

            var productDTO = new ProductDTO
            {
                Id = 1,
                Name = "Updated Product",
                Description = "Updated Description",
                Price = 150m,
                Stock = 20
            };

            var product = new Product(1, "Updated Product", "Updated Description", 150m, 20, "updated.jpg", 1);

            mockMediator.Setup(m => m.Send(It.IsAny<IRequest<Product>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(product);

            // Act
            await service.Update(productDTO);

            // Assert
            mockMediator.Verify(m => m.Send(It.IsAny<IRequest<Product>>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        #endregion
    }
}
