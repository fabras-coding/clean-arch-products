using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using CleanArch_Products.Domain.Entities;
using CleanArch_Products.Domain.Interfaces;
using CleanArch_Products.Infra.Data.Context;
using CleanArch_Products.Infra.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CleanArch_Products.Domain.Tests
{
    public class ProductRepositoryTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        #region CreateAsync Tests

        [Fact(DisplayName = "CreateAsync - Should create product successfully with valid data")]
        public async Task CreateAsync_WithValidProduct_ShouldCreateProductSuccessfully()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new ProductRepository(context);
            
            var category = new Category(1, "Electronics");
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var product = new Product("Laptop", "High-performance laptop", 1200.00m, 5, "laptop.jpg", 1);

            // Act
            var result = await repository.CreateAsync(product);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Laptop");
            result.Price.Should().Be(1200.00m);
            result.Stock.Should().Be(5);
            
            var savedProduct = await context.Products.FirstOrDefaultAsync(p => p.Name == "Laptop");
            savedProduct.Should().NotBeNull();
        }

        [Fact(DisplayName = "CreateAsync - Should persist product to database")]
        public async Task CreateAsync_ShouldPersistProductToDatabase()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new ProductRepository(context);
            
            var category = new Category(1, "Books");
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var product = new Product("C# Programming", "Learn C# from basics", 45.99m, 20, "csharp.jpg", 1);

            // Act
            await repository.CreateAsync(product);

            // Assert
            var dbProduct = context.Products.FirstOrDefault(p => p.Name == "C# Programming");
            dbProduct.Should().NotBeNull();
            dbProduct.Description.Should().Be("Learn C# from basics");
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact(DisplayName = "GetByIdAsync - Should return product when ID exists")]
        public async Task GetByIdAsync_WithValidId_ShouldReturnProduct()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new ProductRepository(context);
            
            var category = new Category(1, "Electronics");
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var product = new Product(1, "Monitor", "4K Monitor", 350.00m, 10, "monitor.jpg", 1);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Monitor");
        }

        [Fact(DisplayName = "GetByIdAsync - Should return null when product ID does not exist")]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new ProductRepository(context);

            // Act
            var result = await repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region GetProductAndCategoryAsync Tests

        [Fact(DisplayName = "GetProductAndCategoryAsync - Should return product with category included")]
        public async Task GetProductAndCategoryAsync_WithValidId_ShouldReturnProductWithCategory()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new ProductRepository(context);
            
            var category = new Category(1, "Smartphones");
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var product = new Product(1, "iPhone 15", "Latest iPhone", 999.00m, 15, "iphone.jpg", 1);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetProductAndCategoryAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("iPhone 15");
            result.Category.Should().NotBeNull();
            result.Category.Name.Should().Be("Smartphones");
        }

        [Fact(DisplayName = "GetProductAndCategoryAsync - Should return null when product does not exist")]
        public async Task GetProductAndCategoryAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new ProductRepository(context);

            // Act
            var result = await repository.GetProductAndCategoryAsync(999);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region GetProductsAsync Tests

        [Fact(DisplayName = "GetProductsAsync - Should return all products")]
        public async Task GetProductsAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new ProductRepository(context);
            
            var category = new Category(1, "Electronics");
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var products = new List<Product>
            {
                new Product(1, "Product 1", "Description 1", 100m, 5, "prod1.jpg", 1),
                new Product(2, "Product 2", "Description 2", 200m, 10, "prod2.jpg", 1),
                new Product(3, "Product 3", "Description 3", 300m, 15, "prod3.jpg", 1)
            };
            
            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetProductsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().Contain(p => p.Name == "Product 1");
            result.Should().Contain(p => p.Name == "Product 2");
            result.Should().Contain(p => p.Name == "Product 3");
        }

        [Fact(DisplayName = "GetProductsAsync - Should return empty list when no products exist")]
        public async Task GetProductsAsync_WithNoProducts_ShouldReturnEmptyList()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new ProductRepository(context);

            // Act
            var result = await repository.GetProductsAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        #endregion

        #region GetProductsByCategoryIdAsync Tests

        [Fact(DisplayName = "GetProductsByCategoryIdAsync - Should return products for specific category")]
        public async Task GetProductsByCategoryIdAsync_WithValidCategoryId_ShouldReturnProductsInCategory()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new ProductRepository(context);
            
            var category1 = new Category(1, "Electronics");
            var category2 = new Category(2, "Books");
            context.Categories.AddRange(category1, category2);
            await context.SaveChangesAsync();

            var products = new List<Product>
            {
                new Product(1, "Phone", "Mobile phone", 500m, 5, "phone.jpg", 1),
                new Product(2, "Laptop", "Gaming laptop", 1500m, 3, "laptop.jpg", 1),
                new Product(3, "Novel", "Fiction book", 25m, 50, "book.jpg", 2)
            };
            
            context.Products.AddRange(products);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetProductsByCategoryIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(p => p.Name == "Phone");
            result.Should().Contain(p => p.Name == "Laptop");
            result.Should().NotContain(p => p.Name == "Novel");
        }

        [Fact(DisplayName = "GetProductsByCategoryIdAsync - Should return empty list when category has no products")]
        public async Task GetProductsByCategoryIdAsync_WithNonExistingProducts_ShouldReturnEmptyList()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new ProductRepository(context);
            
            var category = new Category(1, "Electronics");
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetProductsByCategoryIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        #endregion

        #region UpdateAsync Tests

        [Fact(DisplayName = "UpdateAsync - Should update product successfully")]
        public async Task UpdateAsync_WithValidProduct_ShouldUpdateProductSuccessfully()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new ProductRepository(context);
            
            var category = new Category(1, "Electronics");
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var product = new Product(1, "Original Name", "Original Description", 100m, 5, "original.jpg", 1);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            product = context.Products.FirstOrDefault(p => p.Id == 1);
            product.Update("Updated Name", "Updated Description", 150m, 10, "updated.jpg", 1);

            // Act
            var result = await repository.UpdateAsync(product);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Updated Name");
            result.Description.Should().Be("Updated Description");
            result.Price.Should().Be(150m);
            result.Stock.Should().Be(10);

            var dbProduct = context.Products.FirstOrDefault(p => p.Id == 1);
            dbProduct.Name.Should().Be("Updated Name");
        }

        #endregion

        #region RemoveAsync Tests

        [Fact(DisplayName = "RemoveAsync - Should remove product successfully")]
        public async Task RemoveAsync_WithValidProduct_ShouldRemoveProductSuccessfully()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new ProductRepository(context);
            
            var category = new Category(1, "Electronics");
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var product = new Product(1, "Product to Remove", "Description", 100m, 5, "remove.jpg", 1);
            context.Products.Add(product);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.RemoveAsync(product);

            // Assert
            result.Should().NotBeNull();
            var dbProduct = context.Products.FirstOrDefault(p => p.Id == 1);
            dbProduct.Should().BeNull();
        }

        [Fact(DisplayName = "RemoveAsync - Should not affect other products when removing one")]
        public async Task RemoveAsync_ShouldNotAffectOtherProducts()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new ProductRepository(context);
            
            var category = new Category(1, "Electronics");
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var product1 = new Product(1, "Product 1", "Description 1", 100m, 5, "prod1.jpg", 1);
            var product2 = new Product(2, "Product 2", "Description 2", 200m, 10, "prod2.jpg", 1);
            context.Products.AddRange(product1, product2);
            await context.SaveChangesAsync();

            // Act
            await repository.RemoveAsync(product1);

            // Assert
            var remainingProducts = context.Products.ToList();
            remainingProducts.Should().HaveCount(1);
            remainingProducts.First().Id.Should().Be(2);
        }

        #endregion
    }
}
