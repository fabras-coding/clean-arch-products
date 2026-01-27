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
    public class CategoryRepositoryTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        #region Create Tests

        [Fact(DisplayName = "Create - Should create category successfully with valid data")]
        public async Task Create_WithValidCategory_ShouldCreateCategorySuccessfully()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CategoryRepository(context);
            var category = new Category("Electronics");

            // Act
            var result = await repository.Create(category);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Electronics");

            var savedCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Electronics");
            savedCategory.Should().NotBeNull();
        }

        [Fact(DisplayName = "Create - Should persist category to database")]
        public async Task Create_ShouldPersistCategoryToDatabase()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CategoryRepository(context);
            var category = new Category("Books");

            // Act
            await repository.Create(category);

            // Assert
            var dbCategory = context.Categories.FirstOrDefault(c => c.Name == "Books");
            dbCategory.Should().NotBeNull();
            dbCategory.Name.Should().Be("Books");
        }

        [Fact(DisplayName = "Create - Should return the created category object")]
        public async Task Create_ShouldReturnCreatedCategory()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CategoryRepository(context);
            var category = new Category("Toys");

            // Act
            var result = await repository.Create(category);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Toys");
            result.Should().BeOfType<Category>();
        }

        #endregion

        #region GetByIdAsync Tests

        [Fact(DisplayName = "GetByIdAsync - Should return category when ID exists")]
        public async Task GetByIdAsync_WithValidId_ShouldReturnCategory()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CategoryRepository(context);

            var category = new Category(1, "Sports");
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetByIdAsync(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Sports");
        }

        [Fact(DisplayName = "GetByIdAsync - Should return null when category ID does not exist")]
        public async Task GetByIdAsync_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CategoryRepository(context);

            // Act
            var result = await repository.GetByIdAsync(999);

            // Assert
            result.Should().BeNull();
        }

        [Fact(DisplayName = "GetByIdAsync - Should return null with negative ID")]
        public async Task GetByIdAsync_WithNegativeId_ShouldReturnNull()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CategoryRepository(context);

            // Act
            var result = await repository.GetByIdAsync(-1);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region GetCategoriesAsync Tests

        [Fact(DisplayName = "GetCategoriesAsync - Should return all categories")]
        public async Task GetCategoriesAsync_ShouldReturnAllCategories()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CategoryRepository(context);

            var categories = new List<Category>
            {
                new Category(1, "Electronics"),
                new Category(2, "Books"),
                new Category(3, "Clothing")
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetCategoriesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().Contain(c => c.Name == "Electronics");
            result.Should().Contain(c => c.Name == "Books");
            result.Should().Contain(c => c.Name == "Clothing");
        }

        [Fact(DisplayName = "GetCategoriesAsync - Should return empty list when no categories exist")]
        public async Task GetCategoriesAsync_WithNoCategories_ShouldReturnEmptyList()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CategoryRepository(context);

            // Act
            var result = await repository.GetCategoriesAsync();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact(DisplayName = "GetCategoriesAsync - Should return categories in correct order")]
        public async Task GetCategoriesAsync_ShouldReturnAllCategoriesInOrder()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CategoryRepository(context);

            var categories = new List<Category>
            {
                new Category(1, "First"),
                new Category(2, "Second"),
                new Category(3, "Third")
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetCategoriesAsync();

            // Assert
            result.Should().HaveCount(3);
            result.First().Name.Should().Be("First");
            result.Skip(1).First().Name.Should().Be("Second");
            result.Last().Name.Should().Be("Third");
        }

        #endregion

        #region Update Tests

        [Fact(DisplayName = "Update - Should update category successfully")]
        public async Task Update_WithValidCategory_ShouldUpdateCategorySuccessfully()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CategoryRepository(context);

            var category = new Category(1, "Original Name");
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            // Recupera a categoria do banco para evitar conflito de rastreamento
            var categoryToUpdate = await context.Categories.FirstOrDefaultAsync(c => c.Id == 1);
            categoryToUpdate.Update("Updated Name");  // Atualiza o nome da instância rastreada

            // Act
            var result = await repository.Update(categoryToUpdate);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be("Updated Name");

            var dbCategory = context.Categories.FirstOrDefault(c => c.Id == 1);
            dbCategory.Name.Should().Be("Updated Name");
        }

       

        [Fact(DisplayName = "Update - Should only update specified category")]
        public async Task Update_ShouldOnlyUpdateSpecificCategory()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CategoryRepository(context);

            var category1 = new Category(1, "Category 1");
            var category2 = new Category(2, "Category 2");
            context.Categories.AddRange(category1, category2);
            await context.SaveChangesAsync();

            var updatedCategory1 = context.Categories.FirstOrDefault(c => c.Id == 1);
            updatedCategory1.Update("Updated Category 1");

            // Act
            await repository.Update(updatedCategory1);

            // Assert
            var dbCategory1 = context.Categories.FirstOrDefault(c => c.Id == 1);
            var dbCategory2 = context.Categories.FirstOrDefault(c => c.Id == 2);

            dbCategory1.Name.Should().Be("Updated Category 1");
            dbCategory2.Name.Should().Be("Category 2");
        }

        #endregion

        #region Remove Tests

        [Fact(DisplayName = "Remove - Should remove category successfully")]
        public async Task Remove_WithValidCategory_ShouldRemoveCategorySuccessfully()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CategoryRepository(context);

            var category = new Category(1, "Category to Remove");
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.Remove(category);

            // Assert
            result.Should().NotBeNull();
            var dbCategory = context.Categories.FirstOrDefault(c => c.Id == 1);
            dbCategory.Should().BeNull();
        }

        [Fact(DisplayName = "Remove - Should persist deletion to database")]
        public async Task Remove_ShouldPersistDeletionToDatabase()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CategoryRepository(context);

            var category = new Category(1, "Temporary Category");
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            // Act
            await repository.Remove(category);

            // Assert
            var allCategories = context.Categories.ToList();
            allCategories.Should().NotContain(c => c.Name == "Temporary Category");
        }

        [Fact(DisplayName = "Remove - Should not affect other categories")]
        public async Task Remove_ShouldNotAffectOtherCategories()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var repository = new CategoryRepository(context);

            var category1 = new Category(1, "Category 1");
            var category2 = new Category(2, "Category 2");
            context.Categories.AddRange(category1, category2);
            await context.SaveChangesAsync();

            // Act
            await repository.Remove(category1);

            // Assert
            var remainingCategories = context.Categories.ToList();
            remainingCategories.Should().HaveCount(1);
            remainingCategories.First().Id.Should().Be(2);
            remainingCategories.First().Name.Should().Be("Category 2");
        }

        #endregion
    }
}
