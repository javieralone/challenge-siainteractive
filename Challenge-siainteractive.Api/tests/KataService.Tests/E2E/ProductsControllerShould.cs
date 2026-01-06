using Challenge.Api;
using Challenge.Commands.Categories.Create;
using Challenge.Commands.Products.AssignCategory;
using Challenge.Commands.Products.Create;
using Challenge.Commands.Products.RemoveCategory;
using Challenge.Commands.Products.Update;
using Challenge.Commands.Products.UploadImage;
using Challenge.Queries.Products.GetAll;
using Challenge.Queries.Products.GetById;
using Challenge.Tests.Api;
using Challenge.Tests.BogusData;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Xunit;

namespace KataService.Tests.E2E;

[Collection("IntegrationTests")]
public class ProductsControllerShould : IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;

    public ProductsControllerShould(CustomWebApplicationFactory<Startup> factory)
    {
        _client = factory.CreateClient();
        Task.WaitAll(factory.RespawnDbContext());
    }

    [Fact]
    public async Task BeCreated()
    {
        // Arrange
        var product = BogusProductGenerator.GetProduct();

        var createProductRequest = new
        {
            product.Name,
            product.Description
        };

        // Act
        var clientResponse = await _client.PostAsync($"api/v1/Products", new StringContent(JsonConvert.SerializeObject(createProductRequest), Encoding.UTF8,
                        "application/json"));

        // Assert
        clientResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await clientResponse.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<CreateProductCommandResponse>(json);

        result!.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task BeUpdated()
    {
        // Arrange
        var originalProduct = BogusProductGenerator.GetProduct();
        var updatedProduct = BogusProductGenerator.GetProduct();

        var createProductRequest = new
        {
            originalProduct.Name,
            originalProduct.Description
        };

        var createResponse = await _client.PostAsync($"api/v1/Products", new StringContent(JsonConvert.SerializeObject(createProductRequest), Encoding.UTF8,
                        "application/json"));
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var createJson = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonConvert.DeserializeObject<CreateProductCommandResponse>(createJson);
        var productId = createResult!.Id;

        var updateProductRequest = new
        {
            Id = productId,
            updatedProduct.Name,
            updatedProduct.Description
        };

        // Act
        var updateResponse = await _client.PutAsync($"api/v1/Products", new StringContent(JsonConvert.SerializeObject(updateProductRequest), Encoding.UTF8,
                        "application/json"));

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updateJson = await updateResponse.Content.ReadAsStringAsync();
        var updateResult = JsonConvert.DeserializeObject<UpdateProductCommandResponse>(updateJson);

        updateResult!.Id.Should().Be(productId);
    }

    [Fact]
    public async Task AssignCategory_ShouldAssignCategoryToProduct()
    {
        // Arrange
        var product = BogusProductGenerator.GetProduct();
        var category = BogusCategoryGenerator.GetCategory();

        // Create product
        var createProductRequest = new
        {
            product.Name,
            product.Description
        };

        var createProductResponse = await _client.PostAsync($"api/v1/Products", new StringContent(JsonConvert.SerializeObject(createProductRequest), Encoding.UTF8,
                        "application/json"));
        createProductResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var productJson = await createProductResponse.Content.ReadAsStringAsync();
        var productResult = JsonConvert.DeserializeObject<CreateProductCommandResponse>(productJson);
        var productId = productResult!.Id;

        // Create category
        var createCategoryRequest = new
        {
            category.Name
        };

        var createCategoryResponse = await _client.PostAsync($"api/v1/Categories", new StringContent(JsonConvert.SerializeObject(createCategoryRequest), Encoding.UTF8,
                        "application/json"));
        createCategoryResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var categoryJson = await createCategoryResponse.Content.ReadAsStringAsync();
        var categoryResult = JsonConvert.DeserializeObject<CreateCategoryCommandResponse>(categoryJson);
        var categoryId = categoryResult!.Id;

        // Act
        var assignResponse = await _client.PostAsync($"api/v1/Products/{productId}/categories/{categoryId}", null);

        // Assert
        assignResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var assignJson = await assignResponse.Content.ReadAsStringAsync();
        var assignResult = JsonConvert.DeserializeObject<AssignCategoryToProductCommandResponse>(assignJson);

        assignResult!.ProductId.Should().Be(productId);
        assignResult.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public async Task RemoveCategory_ShouldRemoveCategoryFromProduct()
    {
        // Arrange
        var product = BogusProductGenerator.GetProduct();
        var category = BogusCategoryGenerator.GetCategory();

        // Create product
        var createProductRequest = new
        {
            product.Name,
            product.Description
        };

        var createProductResponse = await _client.PostAsync($"api/v1/Products", new StringContent(JsonConvert.SerializeObject(createProductRequest), Encoding.UTF8,
                        "application/json"));
        createProductResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var productJson = await createProductResponse.Content.ReadAsStringAsync();
        var productResult = JsonConvert.DeserializeObject<CreateProductCommandResponse>(productJson);
        var productId = productResult!.Id;

        // Create category
        var createCategoryRequest = new
        {
            category.Name
        };

        var createCategoryResponse = await _client.PostAsync($"api/v1/Categories", new StringContent(JsonConvert.SerializeObject(createCategoryRequest), Encoding.UTF8,
                        "application/json"));
        createCategoryResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var categoryJson = await createCategoryResponse.Content.ReadAsStringAsync();
        var categoryResult = JsonConvert.DeserializeObject<CreateCategoryCommandResponse>(categoryJson);
        var categoryId = categoryResult!.Id;

        // Assign category first
        var assignResponse = await _client.PostAsync($"api/v1/Products/{productId}/categories/{categoryId}", null);
        assignResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act
        var removeResponse = await _client.DeleteAsync($"api/v1/Products/{productId}/categories/{categoryId}");

        // Assert
        removeResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var removeJson = await removeResponse.Content.ReadAsStringAsync();
        var removeResult = JsonConvert.DeserializeObject<RemoveCategoryFromProductCommandResponse>(removeJson);

        removeResult!.ProductId.Should().Be(productId);
        removeResult.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public async Task UploadImage_ShouldUploadImageSuccessfully()
    {
        // Arrange
        var product = BogusProductGenerator.GetProduct();

        // Create product
        var createProductRequest = new
        {
            product.Name,
            product.Description
        };

        var createProductResponse = await _client.PostAsync($"api/v1/Products", new StringContent(JsonConvert.SerializeObject(createProductRequest), Encoding.UTF8,
                        "application/json"));
        createProductResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var productJson = await createProductResponse.Content.ReadAsStringAsync();
        var productResult = JsonConvert.DeserializeObject<CreateProductCommandResponse>(productJson);
        var productId = productResult!.Id;

        // Create a test image file
        var imageBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 }; // JPEG header
        var imageContent = new ByteArrayContent(imageBytes);
        imageContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

        var multipartContent = new MultipartFormDataContent();
        multipartContent.Add(imageContent, "file", "test-image.jpg");

        // Act
        var uploadResponse = await _client.PostAsync($"api/v1/Products/{productId}/image", multipartContent);

        // Assert
        uploadResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var uploadJson = await uploadResponse.Content.ReadAsStringAsync();
        var uploadResult = JsonConvert.DeserializeObject<UploadProductImageCommandResponse>(uploadJson);

        uploadResult!.ProductId.Should().Be(productId);
        uploadResult.ImageUrl.Should().NotBeNullOrEmpty();
        uploadResult.ImageUrl.Should().StartWith("/images/products/");
    }

    [Fact]
    public async Task GetById_ShouldReturnProductSuccessfully()
    {
        // Arrange
        var product = BogusProductGenerator.GetProductWithImage();

        var createProductRequest = new
        {
            product.Name,
            product.Description
        };

        var createResponse = await _client.PostAsync($"api/v1/Products", new StringContent(JsonConvert.SerializeObject(createProductRequest), Encoding.UTF8,
                        "application/json"));
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var createJson = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonConvert.DeserializeObject<CreateProductCommandResponse>(createJson);
        var productId = createResult!.Id;

        // Act
        var getResponse = await _client.GetAsync($"api/v1/Products/{productId}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getJson = await getResponse.Content.ReadAsStringAsync();
        var getResult = JsonConvert.DeserializeObject<GetProductByIdQueryResponse>(getJson);

        getResult!.Id.Should().Be(productId);
        getResult.Name.Should().Be(product.Name);
        getResult.Description.Should().Be(product.Description);
    }

    [Fact]
    public async Task GetAll_ShouldReturnPaginatedProducts()
    {
        // Arrange
        var product1 = BogusProductGenerator.GetProduct();
        var product2 = BogusProductGenerator.GetProduct();

        // Create first product
        var createProduct1Request = new { product1.Name, product1.Description };
        var createResponse1 = await _client.PostAsync($"api/v1/Products", new StringContent(JsonConvert.SerializeObject(createProduct1Request), Encoding.UTF8,
                        "application/json"));
        createResponse1.StatusCode.Should().Be(HttpStatusCode.OK);

        // Create second product
        var createProduct2Request = new { product2.Name, product2.Description };
        var createResponse2 = await _client.PostAsync($"api/v1/Products", new StringContent(JsonConvert.SerializeObject(createProduct2Request), Encoding.UTF8,
                        "application/json"));
        createResponse2.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act
        var getAllResponse = await _client.GetAsync($"api/v1/Products?pageNumber=1&recordsPerPage=10");

        // Assert
        getAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getAllJson = await getAllResponse.Content.ReadAsStringAsync();
        var getAllResult = JsonConvert.DeserializeObject<GetProductsQueryResponse>(getAllJson);

        getAllResult!.Result.Should().NotBeNull();
        getAllResult.Result.TotalRecords.Should().BeGreaterThanOrEqualTo(2);
        getAllResult.Result.Results.Should().NotBeNull();
        getAllResult.Result.Results.Count.Should().BeGreaterThanOrEqualTo(2);
        getAllResult.Result.PageNumber.Should().Be(1);
        getAllResult.Result.RecordsPerPage.Should().Be(10);
    }
}

