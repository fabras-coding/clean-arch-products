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
using CleanArch_Products.Domain.Entities;
using CleanArch_Products.Domain.Interfaces;
using Xunit;

namespace CleanArch_Products.Domain.Tests
{
    public class CategoryServiceTests
    {
        private Mock<ICategoryRepository> GetMockCategoryRepository()
        {
            return new Mock<ICategoryRepository>();
        }

        private Mock<IMapper> GetMockMapper()
        {
            return new Mock<IMapper>();
        }

        #region Add Tests

        [Fact(DisplayName = "Add - Should add category successfully with valid DTO")]
        public async Task Add_WithValidCreateCategoryDTO_ShouldAddCategorySuccessfully()
        {
            // Arrange
            var mockRepository = GetMockCategoryRepository();
            var mockMapper = GetMockMapper();

            var service = new CategoryService(mockRepository.Object, mockMapper.Object);
            
            var createCategoryDTO = new CreateCategoryDTO
            {
                Name = "New Category"
            };

            var category = new Category("New Category");

            mockMapper.Setup(m => m.Map<Category>(It.IsAny<CreateCategoryDTO>()))
                .Returns(category);
            
            mockRepository.Setup(r => r.Create(It.IsAny<Category>()))
                .ReturnsAsync(category);

            // Act
            await service.Add(createCategoryDTO);

            // Assert
            mockRepository.Verify(r => r.Create(It.IsAny<Category>()), Times.Once);
            mockMapper.Verify(m => m.Map<Category>(createCategoryDTO), Times.Once);
        }

        [Fact(DisplayName = "Add - Should map CreateCategoryDTO to Category entity")]
        public async Task Add_ShouldMapDTOToEntity()
        {
            // Arrange
            var mockRepository = GetMockCategoryRepository();
            var mockMapper = GetMockMapper();

            var service = new CategoryService(mockRepository.Object, mockMapper.Object);
            
            var createCategoryDTO = new CreateCategoryDTO
            {
                Name = "Electronics"
            };

            var category = new Category("Electronics");

            mockMapper.Setup(m => m.Map<Category>(createCategoryDTO))
                .Returns(category);
            
            mockRepository.Setup(r => r.Create(It.IsAny<Category>()))
                .ReturnsAsync(category);

            // Act
            await service.Add(createCategoryDTO);

            // Assert
            mockMapper.Verify(m => m.Map<Category>(createCategoryDTO), Times.Once);
        }

        [Fact(DisplayName = "Add - Should call repository Create method")]
        public async Task Add_ShouldCallRepositoryCreateMethod()
        {
            // Arrange
            var mockRepository = GetMockCategoryRepository();
            var mockMapper = GetMockMapper();

            var service = new CategoryService(mockRepository.Object, mockMapper.Object);
            
            var createCategoryDTO = new CreateCategoryDTO
            {
                Name = "Books"
            };

            var category = new Category("Books");

            mockMapper.Setup(m => m.Map<Category>(It.IsAny<CreateCategoryDTO>()))
                .Returns(category);
            
            mockRepository.Setup(r => r.Create(It.IsAny<Category>()))
                .ReturnsAsync(category);

            // Act
            await service.Add(createCategoryDTO);

            // Assert
            mockRepository.Verify(r => r.Create(It.IsAny<Category>()), Times.Once);
        }

        #endregion

        #region GetById Tests

        [Fact(DisplayName = "GetById - Should return category DTO when category exists")]
        public async Task GetById_WithValidId_ShouldReturnCategoryDTO()
        {
            // Arrange
            var mockRepository = GetMockCategoryRepository();
            var mockMapper = GetMockMapper();

            var service = new CategoryService(mockRepository.Object, mockMapper.Object);
            
            var category = new Category(1, "Sports");
            var categoryDTO = new CategoryDTO
            {
                Id = 1,
                Name = "Sports"
            };

            mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(category);
            
            mockMapper.Setup(m => m.Map<CategoryDTO>(It.IsAny<Category>()))
                .Returns(categoryDTO);

            // Act
            var result = await service.GetById(1);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("Sports");
        }

        [Fact(DisplayName = "GetById - Should call repository GetByIdAsync method")]
        public async Task GetById_ShouldCallRepositoryGetByIdAsync()
        {
            // Arrange
            var mockRepository = GetMockCategoryRepository();
            var mockMapper = GetMockMapper();

            var service = new CategoryService(mockRepository.Object, mockMapper.Object);
            
            var category = new Category(1, "Gaming");
            var categoryDTO = new CategoryDTO { Id = 1, Name = "Gaming" };

            mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(category);
            
            mockMapper.Setup(m => m.Map<CategoryDTO>(It.IsAny<Category>()))
                .Returns(categoryDTO);

            // Act
            await service.GetById(1);

            // Assert
            mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact(DisplayName = "GetById - Should map category to CategoryDTO")]
        public async Task GetById_ShouldMapEntityToDTO()
        {
            // Arrange
            var mockRepository = GetMockCategoryRepository();
            var mockMapper = GetMockMapper();

            var service = new CategoryService(mockRepository.Object, mockMapper.Object);
            
            var category = new Category(1, "Clothing");
            var categoryDTO = new CategoryDTO { Id = 1, Name = "Clothing" };

            mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(category);
            
            mockMapper.Setup(m => m.Map<CategoryDTO>(category))
                .Returns(categoryDTO);

            // Act
            var result = await service.GetById(1);

            // Assert
            mockMapper.Verify(m => m.Map<CategoryDTO>(category), Times.Once);
            result.Should().Be(categoryDTO);
        }

        [Fact(DisplayName = "GetById - Should return null when category does not exist")]
        public async Task GetById_WithInvalidId_ShouldReturnNull()
        {
            // Arrange
            var mockRepository = GetMockCategoryRepository();
            var mockMapper = GetMockMapper();

            var service = new CategoryService(mockRepository.Object, mockMapper.Object);

            mockRepository.Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Category)null);
            
            mockMapper.Setup(m => m.Map<CategoryDTO>(null))
                .Returns((CategoryDTO)null);

            // Act
            var result = await service.GetById(999);

            // Assert
            result.Should().BeNull();
        }

        #endregion

        #region GetCategories Tests

        [Fact(DisplayName = "GetCategories - Should return all categories")]
        public async Task GetCategories_ShouldReturnAllCategories()
        {
            // Arrange
            var mockRepository = GetMockCategoryRepository();
            var mockMapper = GetMockMapper();

            var service = new CategoryService(mockRepository.Object, mockMapper.Object);
            
            var categories = new List<Category>
            {
                new Category(1, "Category 1"),
                new Category(2, "Category 2"),
                new Category(3, "Category 3")
            };

            var categoryDTOs = new List<CategoryDTO>
            {
                new CategoryDTO { Id = 1, Name = "Category 1" },
                new CategoryDTO { Id = 2, Name = "Category 2" },
                new CategoryDTO { Id = 3, Name = "Category 3" }
            };

            mockRepository.Setup(r => r.GetCategoriesAsync())
                .ReturnsAsync(categories);
            
            mockMapper.Setup(m => m.Map<IEnumerable<CategoryDTO>>(It.IsAny<IEnumerable<Category>>()))
                .Returns(categoryDTOs);

            // Act
            var result = await service.GetCategories();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(3);
            result.Should().Contain(c => c.Name == "Category 1");
            result.Should().Contain(c => c.Name == "Category 2");
            result.Should().Contain(c => c.Name == "Category 3");
        }

        [Fact(DisplayName = "GetCategories - Should return empty list when no categories exist")]
        public async Task GetCategories_WithNoCategories_ShouldReturnEmptyList()
        {
            // Arrange
            var mockRepository = GetMockCategoryRepository();
            var mockMapper = GetMockMapper();

            var service = new CategoryService(mockRepository.Object, mockMapper.Object);

            mockRepository.Setup(r => r.GetCategoriesAsync())
                .ReturnsAsync(new List<Category>());
            
            mockMapper.Setup(m => m.Map<IEnumerable<CategoryDTO>>(It.IsAny<IEnumerable<Category>>()))
                .Returns(new List<CategoryDTO>());

            // Act
            var result = await service.GetCategories();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
        }

        [Fact(DisplayName = "GetCategories - Should call repository GetCategoriesAsync method")]
        public async Task GetCategories_ShouldCallRepositoryGetCategoriesAsync()
        {
            // Arrange
            var mockRepository = GetMockCategoryRepository();
            var mockMapper = GetMockMapper();

            var service = new CategoryService(mockRepository.Object, mockMapper.Object);

            mockRepository.Setup(r => r.GetCategoriesAsync())
                .ReturnsAsync(new List<Category>());
            
            mockMapper.Setup(m => m.Map<IEnumerable<CategoryDTO>>(It.IsAny<IEnumerable<Category>>()))
                .Returns(new List<CategoryDTO>());

            // Act
            await service.GetCategories();

            // Assert
            mockRepository.Verify(r => r.GetCategoriesAsync(), Times.Once);
        }

        [Fact(DisplayName = "GetCategories - Should map categories to DTOs")]
        public async Task GetCategories_ShouldMapEntitiesToDTOs()
        {
            // Arrange
            var mockRepository = GetMockCategoryRepository();
            var mockMapper = GetMockMapper();

            var service = new CategoryService(mockRepository.Object, mockMapper.Object);
            
            var categories = new List<Category>
            {
                new Category(1, "Electronics")
            };

            var categoryDTOs = new List<CategoryDTO>
            {
                new CategoryDTO { Id = 1, Name = "Electronics" }
            };

            mockRepository.Setup(r => r.GetCategoriesAsync())
                .ReturnsAsync(categories);
            
            mockMapper.Setup(m => m.Map<IEnumerable<CategoryDTO>>(categories))
                .Returns(categoryDTOs);

            // Act
            await service.GetCategories();

            // Assert
            mockMapper.Verify(m => m.Map<IEnumerable<CategoryDTO>>(categories), Times.Once);
        }

        #endregion

        #region Remove Tests

        [Fact(DisplayName = "Remove - Should remove category successfully")]
        public async Task Remove_WithValidId_ShouldRemoveCategorySuccessfully()
        {
            // Arrange
            var mockRepository = GetMockCategoryRepository();
            var mockMapper = GetMockMapper();

            var service = new CategoryService(mockRepository.Object, mockMapper.Object);
            
            var category = new Category(1, "Category to Remove");

            mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(category);
            
            mockRepository.Setup(r => r.Remove(It.IsAny<Category>()))
                .ReturnsAsync(category);

            // Act
            await service.Remove(1);

            // Assert
            mockRepository.Verify(r => r.Remove(It.IsAny<Category>()), Times.Once);
        }

        [Fact(DisplayName = "Remove - Should fetch category before removing")]
        public async Task Remove_ShouldFetchCategoryBeforeRemoving()
        {
            // Arrange
            var mockRepository = GetMockCategoryRepository();
            var mockMapper = GetMockMapper();

            var service = new CategoryService(mockRepository.Object, mockMapper.Object);
            
            var category = new Category(1, "Temp Category");

            mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(category);
            
            mockRepository.Setup(r => r.Remove(It.IsAny<Category>()))
                .ReturnsAsync(category);

            // Act
            await service.Remove(1);

            // Assert
            mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        }

        [Fact(DisplayName = "Remove - Should call repository Remove method")]
        public async Task Remove_ShouldCallRepositoryRemoveMethod()
        {
            // Arrange
            var mockRepository = GetMockCategoryRepository();
            var mockMapper = GetMockMapper();

            var service = new CategoryService(mockRepository.Object, mockMapper.Object);
            
            var category = new Category(1, "Remove This");

            mockRepository.Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(category);
            
            mockRepository.Setup(r => r.Remove(category))
                .ReturnsAsync(category);

            // Act
            await service.Remove(1);

            // Assert
            mockRepository.Verify(r => r.Remove(It.IsAny<Category>()), Times.Once);
        }

        #endregion

        #region Update Tests

        [Fact(DisplayName = "Update - Should update category successfully")]
        public async Task Update_WithValidCategoryDTO_ShouldUpdateCategorySuccessfully()
        {
            // Arrange
            var mockRepository = GetMockCategoryRepository();
            var mockMapper = GetMockMapper();

            var service = new CategoryService(mockRepository.Object, mockMapper.Object);
            
            var categoryDTO = new CategoryDTO
            {
                Id = 1,
                Name = "Updated Category"
            };

            var category = new Category(1, "Updated Category");

            mockMapper.Setup(m => m.Map<Category>(It.IsAny<CategoryDTO>()))
                .Returns(category);
            
            mockRepository.Setup(r => r.Update(It.IsAny<Category>()))
                .ReturnsAsync(category);

            // Act
            await service.Update(categoryDTO);

            // Assert
            mockRepository.Verify(r => r.Update(It.IsAny<Category>()), Times.Once);
        }

        [Fact(DisplayName = "Update - Should map CategoryDTO to Category entity")]
        public async Task Update_ShouldMapDTOToEntity()
        {
            // Arrange
            var mockRepository = GetMockCategoryRepository();
            var mockMapper = GetMockMapper();

            var service = new CategoryService(mockRepository.Object, mockMapper.Object);
            
            var categoryDTO = new CategoryDTO
            {
                Id = 1,
                Name = "New Name"
            };

            var category = new Category(1, "New Name");

            mockMapper.Setup(m => m.Map<Category>(categoryDTO))
                .Returns(category);
            
            mockRepository.Setup(r => r.Update(It.IsAny<Category>()))
                .ReturnsAsync(category);

            // Act
            await service.Update(categoryDTO);

            // Assert
            mockMapper.Verify(m => m.Map<Category>(categoryDTO), Times.Once);
        }

        [Fact(DisplayName = "Update - Should call repository Update method")]
        public async Task Update_ShouldCallRepositoryUpdateMethod()
        {
            // Arrange
            var mockRepository = GetMockCategoryRepository();
            var mockMapper = GetMockMapper();

            var service = new CategoryService(mockRepository.Object, mockMapper.Object);
            
            var categoryDTO = new CategoryDTO
            {
                Id = 1,
                Name = "Updated"
            };

            var category = new Category(1, "Updated");

            mockMapper.Setup(m => m.Map<Category>(It.IsAny<CategoryDTO>()))
                .Returns(category);
            
            mockRepository.Setup(r => r.Update(category))
                .ReturnsAsync(category);

            // Act
            await service.Update(categoryDTO);

            // Assert
            mockRepository.Verify(r => r.Update(It.IsAny<Category>()), Times.Once);
        }

        [Fact(DisplayName = "Update - Should update category with new data")]
        public async Task Update_ShouldUpdateCategoryWithNewData()
        {
            // Arrange
            var mockRepository = GetMockCategoryRepository();
            var mockMapper = GetMockMapper();

            var service = new CategoryService(mockRepository.Object, mockMapper.Object);
            
            var categoryDTO = new CategoryDTO
            {
                Id = 1,
                Name = "Fashion"
            };

            var category = new Category(1, "Fashion");

            mockMapper.Setup(m => m.Map<Category>(categoryDTO))
                .Returns(category);
            
            mockRepository.Setup(r => r.Update(category))
                .ReturnsAsync(category);

            // Act
            await service.Update(categoryDTO);

            // Assert
            mockRepository.Verify(r => r.Update(It.Is<Category>(c => c.Name == "Fashion")), Times.Once);
        }

        #endregion
    }
}
