using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using CleanArch_Products.Domain.Entities;
using CleanArch_Products.Domain.Validation;

namespace CleanArch_Products.Domain.Tests
{
    public class CategoryUnitTest1
    {

        [Fact(DisplayName = "Create Category Object With Valid State")]
        public void CreateCategory_WithValidParameters_ResultObjectValidState()
        {

            Action action = () => new CleanArch_Products.Domain.Entities.Category(1, "Category Name");
            action.Should()
                .NotThrow<CleanArch_Products.Domain.Validation.DomainExceptionValidation>();

        }


        //gerados pelo Copilot

         [Fact]
        public void CreateCategory_WithValidName_ShouldSetName()
        {
            // Arrange
            var name = "Electronics";

            // Act
            var category = new Category(name);

            // Assert
            category.Name.Should().Be(name);
            
        }

        [Fact]
        public void CreateCategory_WithValidIdAndName_ShouldSetProperties()
        {
            // Arrange
            var id = 1;
            var name = "Books";

            // Act
            var category = new Category(id, name);

            // Assert
            category.Id.Should().Be(id);
            category.Name.Should().Be(name);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void CreateCategory_WithInvalidName_ShouldThrowException(string invalidName)
        {
            // Arrange
            Action act = () => new Category(invalidName);

            // Act & Assert
            act.Should().Throw<DomainExceptionValidation>()
                .WithMessage("Invalid name. Name is required.");
        }

        [Fact]
        public void CreateCategory_WithShortName_ShouldThrowException()
        {
            // Arrange
            var shortName = "AB";
            Action act = () => new Category(shortName);

            // Act & Assert
            act.Should().Throw<DomainExceptionValidation>()
                .WithMessage("Invalid name, minimum 3 characters.");
        }

        [Fact]
        public void CreateCategory_WithInvalidId_ShouldThrowException()
        {
            // Arrange
            var invalidId = 0;
            var name = "Toys";
            Action act = () => new Category(invalidId, name);

            // Act & Assert
            act.Should().Throw<DomainExceptionValidation>()
                .WithMessage("invalid Id value. Must be greater than zero");
        }

        [Fact]
        public void UpdateCategory_WithValidName_ShouldUpdateName()
        {
            // Arrange
            var category = new Category("Initial");
            var newName = "Updated";

            // Act
            category.Update(newName);

            // Assert
            category.Name.Should().Be(newName);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void UpdateCategory_WithInvalidName_ShouldThrowException(string invalidName)
        {
            // Arrange
            var category = new Category("Initial");

            // Act
            Action act = () => category.Update(invalidName);

            // Assert
            act.Should().Throw<DomainExceptionValidation>()
                .WithMessage("Invalid name. Name is required.");
        }

        [Fact]
        public void UpdateCategory_WithShortName_ShouldThrowException()
        {
            // Arrange
            var category = new Category("Initial");
            var shortName = "AB";

            // Act
            Action act = () => category.Update(shortName);

            // Assert
            act.Should().Throw<DomainExceptionValidation>()
                .WithMessage("Invalid name, minimum 3 characters.");
        }

    }
}