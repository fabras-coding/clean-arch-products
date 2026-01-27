# Unit Tests Documentation - Clean Architecture Products

## Overview
This document provides a comprehensive explanation of all unit tests created for the ProductRepository, CategoryRepository, ProductService, and CategoryService classes using the **Arrange, Act, Assert (AAA)** pattern.

---

## Table of Contents
1. [ProductRepositoryTests](#productrepositorytests)
2. [CategoryRepositoryTests](#categoryrepositorytests)
3. [ProductServiceTests](#productservicetests)
4. [CategoryServiceTests](#categoryservicetests)

---

## ProductRepositoryTests

Located in: `ProductRepositoryTests.cs`

This test class validates all CRUD operations for the ProductRepository using an in-memory database context.

### Setup
- Uses `DbContextOptionsBuilder` with `UseInMemoryDatabase` for isolated testing
- Creates a new database instance for each test to ensure independence

### Test Methods

#### **CreateAsync Tests**

**Test 1: CreateAsync - Should create product successfully with valid data**
```
Arrange: Create a product object with valid properties
Act: Call CreateAsync() on the repository
Assert: Verify the product is returned and has correct properties (name, price, stock)
```
- Tests basic creation functionality
- Ensures product object is saved correctly

**Test 2: CreateAsync - Should persist product to database**
```
Arrange: Create a product with specific data
Act: Call CreateAsync() and query the database directly
Assert: Verify the product exists in the database context
```
- Ensures persistence layer is working
- Validates SaveChangesAsync() is called correctly

#### **GetByIdAsync Tests**

**Test 3: GetByIdAsync - Should return product when ID exists**
```
Arrange: Create and add a product to the database with ID=1
Act: Call GetByIdAsync(1)
Assert: Verify returned product has correct ID and properties
```
- Tests retrieval by valid ID
- Validates mapping of database entity to object

**Test 4: GetByIdAsync - Should return null when product ID does not exist**
```
Arrange: Database has no products
Act: Call GetByIdAsync(999) with non-existent ID
Assert: Verify method returns null
```
- Tests null handling for missing entities
- Ensures no exceptions are thrown

#### **GetProductAndCategoryAsync Tests**

**Test 5: GetProductAndCategory - Should return product with category included**
```
Arrange: Create a category and product linked to that category
Act: Call GetProductAndCategoryAsync(1)
Assert: Verify returned product includes loaded category details
```
- Tests eager loading of related entities
- Validates Include() with Category navigation property

**Test 6: GetProductAndCategory - Should return null when product does not exist**
```
Arrange: Empty database
Act: Call GetProductAndCategoryAsync(999)
Assert: Verify method returns null
```
- Tests behavior with non-existent entities

#### **GetProductsAsync Tests**

**Test 7: GetProducts - Should return all products**
```
Arrange: Insert 3 products into the database
Act: Call GetProductsAsync()
Assert: Verify all 3 products are returned with correct properties
```
- Tests retrieval of all records
- Validates collection mapping

**Test 8: GetProducts - Should return empty list when no products exist**
```
Arrange: Empty database
Act: Call GetProductsAsync()
Assert: Verify empty collection is returned (not null)
```
- Tests empty collection handling
- Ensures method returns IEnumerable instead of null

#### **GetProductsByCategoryIdAsync Tests**

**Test 9: GetProductsByCategoryId - Should return products for specific category**
```
Arrange: Create 2 products in category 1, 1 product in category 2
Act: Call GetProductsByCategoryIdAsync(1)
Assert: Verify only 2 products are returned, not the product from category 2
```
- Tests filtering by category
- Validates WHERE clause in LINQ query

**Test 10: GetProductsByCategoryId - Should return empty list when category has no products**
```
Arrange: Category exists but has no products
Act: Call GetProductsByCategoryIdAsync(1)
Assert: Verify empty list is returned
```
- Tests filtering with no matching records

#### **UpdateAsync Tests**

**Test 11: Update - Should update product successfully**
```
Arrange: Create a product with original name "Original", update it to "Updated"
Act: Call UpdateAsync() with modified product
Assert: Verify the product in database has new values (name, price, stock)
```
- Tests modification of existing records
- Validates Update() and SaveChangesAsync()

#### **RemoveAsync Tests**

**Test 12: Remove - Should remove product successfully**
```
Arrange: Insert a product with ID=1
Act: Call RemoveAsync(product)
Assert: Verify the product no longer exists in database
```
- Tests deletion functionality
- Ensures record is actually removed

**Test 13: Remove - Should not affect other products when removing one**
```
Arrange: Insert 2 products (ID=1, ID=2)
Act: Call RemoveAsync(product with ID=1)
Assert: Verify only product with ID=2 remains
```
- Tests isolation of delete operations
- Ensures other records are unaffected

---

## CategoryRepositoryTests

Located in: `CategoryRepositoryTests.cs`

This test class validates CRUD operations for CategoryRepository using an in-memory database.

### Test Methods

#### **Create Tests**

**Test 1: Create - Should create category successfully with valid data**
```
Arrange: Create a category with name "Electronics"
Act: Call Create(category)
Assert: Verify category object is returned with correct properties
```
- Tests creation functionality
- Validates return value

**Test 2: Create - Should persist category to database**
```
Arrange: Create a category
Act: Call Create() and query database directly
Assert: Verify category exists in database
```
- Tests persistence to database

**Test 3: Create - Should return the created category object**
```
Arrange: Create category object
Act: Call Create()
Assert: Verify returned object is not null and is of type Category
```
- Tests return value type safety

#### **GetByIdAsync Tests**

**Test 4: GetByIdAsync - Should return category when ID exists**
```
Arrange: Insert category with ID=1
Act: Call GetByIdAsync(1)
Assert: Verify category is returned with correct ID and name
```
- Tests successful retrieval

**Test 5: GetByIdAsync - Should return null when category ID does not exist**
```
Arrange: Empty database
Act: Call GetByIdAsync(999)
Assert: Verify null is returned
```
- Tests null handling

**Test 6: GetByIdAsync - Should return null with negative ID**
```
Arrange: Empty database
Act: Call GetByIdAsync(-1)
Assert: Verify null is returned
```
- Tests edge case with invalid ID

#### **GetCategoriesAsync Tests**

**Test 7: GetCategories - Should return all categories**
```
Arrange: Insert 3 categories
Act: Call GetCategoriesAsync()
Assert: Verify all 3 categories are returned
```
- Tests retrieval of all records

**Test 8: GetCategories - Should return empty list when no categories exist**
```
Arrange: Empty database
Act: Call GetCategoriesAsync()
Assert: Verify empty list (not null) is returned
```
- Tests empty collection handling

**Test 9: GetCategories - Should return categories in correct order**
```
Arrange: Insert 3 categories
Act: Call GetCategoriesAsync()
Assert: Verify order of returned items matches insertion order
```
- Tests ordering/sequencing

#### **Update Tests**

**Test 10: Update - Should update category successfully**
```
Arrange: Create category with name "Original", update to "Updated"
Act: Call Update(updatedCategory)
Assert: Verify database contains updated name
```
- Tests modification

**Test 11: Update - Should persist changes to database**
```
Arrange: Create category
Act: Update category name and call Update()
Assert: Query database to verify changes are persisted
```
- Tests persistence of updates

**Test 12: Update - Should only update specified category**
```
Arrange: Create 2 categories
Act: Update only the first one
Assert: Verify first category changed, second unchanged
```
- Tests isolation of updates

#### **Remove Tests**

**Test 13: Remove - Should remove category successfully**
```
Arrange: Insert category
Act: Call Remove(category)
Assert: Verify category no longer exists in database
```
- Tests deletion

**Test 14: Remove - Should persist deletion to database**
```
Arrange: Insert category
Act: Remove it
Assert: Query database to confirm removal
```
- Tests persistence of deletion

**Test 15: Remove - Should not affect other categories**
```
Arrange: Insert 2 categories
Act: Remove only the first one
Assert: Verify second category still exists
```
- Tests isolation of deletion

---

## ProductServiceTests

Located in: `ProductServiceTests.cs`

This test class validates ProductService business logic using Moq for dependency injection.

### Setup
- Uses Moq to create mocks for: IMapper, IMediator, IMessageBus
- Tests use `.Object` property to pass mock instances to service

### Test Methods

#### **Add Tests**

**Test 1: Add - Should add product successfully**
```
Arrange: Create CreateProductDTO, setup mocks to succeed
Act: Call service.Add(createProductDTO)
Assert: Verify mediator.Send() called once
Assert: Verify messageBus.PublishAsync() called once with "product-created"
```
- Tests end-to-end add workflow
- Validates both mediator and message bus are used

**Test 2: Add - Should map CreateProductDTO to ProductCreateCommand**
```
Arrange: Create DTO with specific values
Act: Call Add()
Assert: Verify mapper.Map<ProductCreateCommand>() was called with the DTO
```
- Tests mapping functionality
- Validates mapper interaction

#### **GetById Tests**

**Test 3: GetById - Should return product DTO when product exists**
```
Arrange: Setup mediator to return product, mapper to return DTO
Act: Call service.GetById(1)
Assert: Verify result contains expected name and price
```
- Tests successful retrieval
- Validates DTO mapping

**Test 4: GetById - Should map product to ProductDTO**
```
Arrange: Setup mocks
Act: Call GetById()
Assert: Verify mapper.Map<ProductDTO>() was called once
```
- Tests mapping is invoked

#### **GetProductAndCategory Tests**

**Test 5: GetProductAndCategory - Should return product with category details**
```
Arrange: Setup mediator and mapper
Act: Call GetProductAndCategory(1)
Assert: Verify product DTO is returned with correct properties
```
- Tests retrieval with relationships

**Test 6: GetProductAndCategory - Should send query to mediator**
```
Arrange: Setup mocks
Act: Call GetProductAndCategory()
Assert: Verify mediator.Send() was called once
```
- Tests mediat or integration

#### **GetProducts Tests**

**Test 7: GetProducts - Should return all products**
```
Arrange: Setup mediator to return 2 products, mapper to map them
Act: Call service.GetProducts()
Assert: Verify 2 ProductDTOs are returned
```
- Tests collection retrieval
- Validates plural mapping

**Test 8: GetProducts - Should return empty list when no products exist**
```
Arrange: Setup mediator to return empty list
Act: Call GetProducts()
Assert: Verify empty IEnumerable is returned
```
- Tests empty collection handling

#### **GetProductsByCategoryId Tests**

**Test 9: GetProductsByCategoryId - Should return products for specific category**
```
Arrange: Setup mediator to return 2 products for category 1
Act: Call GetProductsByCategoryId(1)
Assert: Verify 2 products are returned
```
- Tests filtered retrieval

**Test 10: GetProductsByCategoryId - Should return empty list when category has no products**
```
Arrange: Setup mediator to return empty list
Act: Call GetProductsByCategoryId(999)
Assert: Verify empty collection is returned
```
- Tests empty filtered results

#### **Remove Tests**

**Test 11: Remove - Should remove product successfully**
```
Arrange: Setup mediator
Act: Call Remove(1)
Assert: Verify mediator.Send() called once
```
- Tests deletion through mediator

#### **Update Tests**

**Test 12: Update - Should update product successfully**
```
Arrange: Create ProductDTO, setup mediator
Act: Call Update(productDTO)
Assert: Verify mediator.Send() called once
```
- Tests update through mediator

**Test 13: Update - Should map ProductDTO to ProductUpdateCommand**
```
Arrange: Setup mocks
Act: Call Update()
Assert: Verify mapper.Map<ProductUpdateCommand>() was called
```
- Tests mapping to command

---

## CategoryServiceTests

Located in: `CategoryServiceTests.cs`

This test class validates CategoryService business logic using Moq.

### Setup
- Uses Moq for: ICategoryRepository, IMapper
- Tests validate service logic without database dependencies

### Test Methods

#### **Add Tests**

**Test 1: Add - Should add category successfully**
```
Arrange: Create CreateCategoryDTO, setup repository mock
Act: Call Add(createCategoryDTO)
Assert: Verify repository.Create() called once
```
- Tests add workflow
- Validates repository interaction

**Test 2: Add - Should map CreateCategoryDTO to Category entity**
```
Arrange: Create DTO, setup mocks
Act: Call Add()
Assert: Verify mapper.Map<Category>() was called with the DTO
```
- Tests DTO to entity mapping

**Test 3: Add - Should call repository Create method**
```
Arrange: Setup mocks
Act: Call Add()
Assert: Verify repository.Create() was invoked
```
- Tests repository method invocation

#### **GetById Tests**

**Test 4: GetById - Should return category DTO when category exists**
```
Arrange: Setup repository to return category, mapper to return DTO
Act: Call GetById(1)
Assert: Verify DTO with ID=1 and correct name is returned
```
- Tests successful retrieval
- Validates mapping

**Test 5: GetById - Should call repository GetByIdAsync**
```
Arrange: Setup mocks
Act: Call GetById()
Assert: Verify repository.GetByIdAsync() was called once
```
- Tests repository method invocation

**Test 6: GetById - Should map entity to DTO**
```
Arrange: Setup mocks
Act: Call GetById()
Assert: Verify mapper.Map<CategoryDTO>() was called
```
- Tests mapping invocation

**Test 7: GetById - Should return null when category does not exist**
```
Arrange: Setup repository to return null
Act: Call GetById(999)
Assert: Verify null is returned
```
- Tests null handling

#### **GetCategories Tests**

**Test 8: GetCategories - Should return all categories**
```
Arrange: Setup repository to return 3 categories, mapper to map them
Act: Call GetCategories()
Assert: Verify 3 CategoryDTOs are returned
```
- Tests collection retrieval

**Test 9: GetCategories - Should return empty list when no categories exist**
```
Arrange: Setup repository to return empty list
Act: Call GetCategories()
Assert: Verify empty IEnumerable is returned
```
- Tests empty collection handling

**Test 10: GetCategories - Should call repository GetCategoriesAsync**
```
Arrange: Setup mocks
Act: Call GetCategories()
Assert: Verify repository.GetCategoriesAsync() was called once
```
- Tests repository method invocation

**Test 11: GetCategories - Should map entities to DTOs**
```
Arrange: Setup mocks
Act: Call GetCategories()
Assert: Verify mapper.Map<IEnumerable<CategoryDTO>>() was called
```
- Tests mapping invocation

#### **Remove Tests**

**Test 12: Remove - Should remove category successfully**
```
Arrange: Setup repository to return a category
Act: Call Remove(1)
Assert: Verify repository.Remove() called once
```
- Tests deletion workflow

**Test 13: Remove - Should fetch category before removing**
```
Arrange: Setup repository
Act: Call Remove()
Assert: Verify repository.GetByIdAsync() was called first
```
- Tests loading before deletion

**Test 14: Remove - Should call repository Remove method**
```
Arrange: Setup mocks
Act: Call Remove()
Assert: Verify repository.Remove() was invoked
```
- Tests method invocation

#### **Update Tests**

**Test 15: Update - Should update category successfully**
```
Arrange: Create CategoryDTO, setup mocks
Act: Call Update(categoryDTO)
Assert: Verify repository.Update() called once
```
- Tests update workflow

**Test 16: Update - Should map CategoryDTO to Category entity**
```
Arrange: Create DTO, setup mocks
Act: Call Update()
Assert: Verify mapper.Map<Category>() was called
```
- Tests DTO to entity mapping

**Test 17: Update - Should call repository Update method**
```
Arrange: Setup mocks
Act: Call Update()
Assert: Verify repository.Update() was invoked
```
- Tests method invocation

**Test 18: Update - Should update category with new data**
```
Arrange: Create DTO with new values
Act: Call Update()
Assert: Verify repository.Update() called with entity having new values
```
- Tests data persistence

---

## AAA Pattern Explanation

All tests follow the **Arrange, Act, Assert** pattern:

### **Arrange**
- Set up test data and dependencies
- Create mock objects if needed
- Configure mock return values and expectations

### **Act**
- Execute the method being tested
- Perform the action that will be validated

### **Assert**
- Verify the result matches expectations
- Use FluentAssertions for readable assertions
- Check mock method invocations with `.Verify()`

---

## Test Execution

Run all tests:
```bash
dotnet test CleanArch-Products.Domain.Tests/CleanArch-Products.Domain.Tests.csproj
```

Run specific test class:
```bash
dotnet test CleanArch-Products.Domain.Tests/CleanArch-Products.Domain.Tests.csproj --filter "ClassName"
```

Run specific test method:
```bash
dotnet test CleanArch-Products.Domain.Tests/CleanArch-Products.Domain.Tests.csproj --filter "ClassName.MethodName"
```

---

## Key Testing Concepts Used

- **Arrange, Act, Assert**: Clear test structure
- **In-Memory Database**: Isolated repository tests without real database
- **Moq**: Mock dependencies for service tests
- **FluentAssertions**: Readable assertion syntax
- **xUnit**: Modern testing framework with Fact and Theory attributes
- **Null Handling**: Tests for edge cases and null returns
- **Collection Testing**: Verify empty and populated collections
- **Mock Verification**: Ensure correct method calls and parameters

---

## Summary

- **ProductRepository**: 13 tests covering CRUD operations and queries
- **CategoryRepository**: 15 tests covering CRUD operations
- **ProductService**: 13 tests covering business logic with mocked dependencies
- **CategoryService**: 18 tests covering business logic with mocked dependencies

**Total: 59 comprehensive unit tests**

All tests are designed to be independent, repeatable, and maintainable, following Clean Code principles and unit testing best practices.
