using FluentAssertions;
using CleanArch_Products.Domain.Entities;
using CleanArch_Products.Domain.Validation;

namespace CleanArch_Products.Domain.Tests
{
    public class ProductUnityTests1
    {
        [Fact]
        public void CreateProduct_WithValidParameters_ShouldCreateProduct()
        {
            // Arrange
            var name = "Product Name";
            var description = "Product Description";
            var price = 10.5m;
            var stock = 5;
            var image = "image.png";

            // Act
            var product = new Product(name, description, price, stock, image,1);

            // Assert
            product.Name.Should().Be(name);
            product.Description.Should().Be(description);
            product.Price.Should().Be(price);
            product.Stock.Should().Be(stock);
            product.Image.Should().Be(image);
        }

        [Fact]
        public void CreateProduct_WithValidId_ShouldSetId()
        {
            // Arrange
            var id = 1;
            var name = "Product Name";
            var description = "Product Description";
            var price = 10.5m;
            var stock = 5;
            var image = "image.png";

            // Act
            var product = new Product(id, name, description, price, stock, image, 1);

            // Assert
            product.Id.Should().Be(id);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void CreateProduct_WithInvalidName_ShouldThrowException(string invalidName)
        {
            // Arrange
            Action act = () => new Product(invalidName, "Valid Description", 10, 1, "img.png", 1);

            // Act & Assert
            act.Should().Throw<DomainExceptionValidation>()
                .WithMessage("Invalid name. Name is required.");
        }

        [Fact]
        public void CreateProduct_WithShortName_ShouldThrowException()
        {
            // Arrange
            var shortName = "AB";
            Action act = () => new Product(shortName, "Valid Description", 10, 1, "img.png",1);

            // Act & Assert
            act.Should().Throw<DomainExceptionValidation>()
                .WithMessage("Invalid name, too short. The minimum is 3 characters.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void CreateProduct_WithInvalidDescription_ShouldThrowException(string invalidDescription)
        {
            // Arrange
            Action act = () => new Product("Valid Name", invalidDescription, 10, 1, "img.png", 1);

            // Act & Assert
            act.Should().Throw<DomainExceptionValidation>()
                .WithMessage("invalid description. Description is required.");
        }

        [Fact]
        public void CreateProduct_WithShortDescription_ShouldThrowException()
        {
            // Arrange
            var shortDescription = "Short";
            Action act = () => new Product("Valid Name", shortDescription, 10, 1, "img.png",1);

            // Act & Assert
            act.Should().Throw<DomainExceptionValidation>()
                .WithMessage("Invalid description, too short. The minimum is 10 characters.");
        }

        [Fact]
        public void CreateProduct_WithNegativePrice_ShouldThrowException()
        {
            // Arrange
            Action act = () => new Product("Valid Name", "Valid Description", -1, 1, "img.png",1);

            // Act & Assert
            act.Should().Throw<DomainExceptionValidation>()
                .WithMessage("Invalid value. Price must be greater than zero.");
        }

        [Fact]
        public void CreateProduct_WithNegativeStock_ShouldThrowException()
        {
            // Arrange
            Action act = () => new Product("Valid Name", "Valid Description", 10, -1, "img.png",1);

            // Act & Assert
            act.Should().Throw<DomainExceptionValidation>()
                .WithMessage("Invalid value. Stock must be greater than zero.");
        }

        [Fact]
        public void CreateProduct_WithLongImageName_ShouldThrowException()
        {
            // Arrange
            var longImage = new string('a', 251);
            Action act = () => new Product("Valid Name", "Valid Description", 10, 1, longImage,1);

            // Act & Assert
            act.Should().Throw<DomainExceptionValidation>()
                .WithMessage("Invalid image name, too long. The maximum is 250 characters.");
        }

        [Fact]
        public void CreateProduct_WithInvalidId_ShouldThrowException()
        {
            // Arrange
            Action act = () => new Product(0, "Valid Name", "Valid Description", 10, 1, "img.png",1);

            // Act & Assert
            act.Should().Throw<DomainExceptionValidation>()
                .WithMessage("Invalid id value. Must be greater than zero");
        }

        [Fact]
        public void UpdateProduct_WithValidParameters_ShouldUpdateProduct()
        {
            // Arrange
            var product = new Product("Old Name", "Old Description", 5, 2, "old.png",1);
            var newName = "New Name";
            var newDescription = "New Description";
            var newPrice = 20m;
            var newStock = 10;
            var newImage = "new.png";
            var newCategoryId = 2;

            // Act
            product.Update(newName, newDescription, newPrice, newStock, newImage, newCategoryId);

            // Assert
            product.Name.Should().Be(newName);
            product.Description.Should().Be(newDescription);
            product.Price.Should().Be(newPrice);
            product.Stock.Should().Be(newStock);
            product.Image.Should().Be(newImage);
            product.CategoryId.Should().Be(newCategoryId);
        }

        [Fact]
        public void UpdateProduct_WithInvalidCategoryId_ShouldThrowException()
        {
            // Arrange
            var product = new Product("Name", "Description", 10, 1, "img.png", 1);
            var invalidCategoryId = 0;

            // Act
            Action act = () => product.Update("Name", "Description", 10, 1, "img.png", invalidCategoryId);

            // Assert
            act.Should().Throw<DomainExceptionValidation>()
                .WithMessage("Invalid id value. Must be greater than zero");
        }
    }
}