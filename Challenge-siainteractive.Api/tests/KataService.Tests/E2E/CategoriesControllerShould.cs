using Challenge.Api;
using Challenge.Commands.Categories.Create;
using Challenge.Commands.Categories.Update;
using Challenge.Queries.Categories.GetAll;
using Challenge.Queries.Categories.GetById;
using Challenge.Tests.Api;
using Challenge.Tests.BogusData;
using FluentAssertions;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Xunit;

namespace Challenge.Tests.E2E;

[Collection("IntegrationTests")]
public class CategoriesControllerShould :  IClassFixture<CustomWebApplicationFactory<Startup>>
{
    private readonly HttpClient _client;

    public CategoriesControllerShould(CustomWebApplicationFactory<Startup> factory)
    {
        _client = factory.CreateClient();
        Task.WaitAll(factory.RespawnDbContext());
    }
    [Fact]
    public async Task BeCreated()
    {
        // Arrange
        var category = BogusCategoryGenerator.GetCategory();

        var createCategoryRequest = new 
        {
            category.Name
        };

        // Act
        var clientResponse = await _client.PostAsync($"api/v1/Categories", new StringContent(JsonConvert.SerializeObject(createCategoryRequest), Encoding.UTF8,
                        "application/json"));

        // Assert
        clientResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await clientResponse.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<CreateCategoryCommandResponse>(json);

        result!.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task BeUpdated()
    {
        // Arrange
        var originalCategory = BogusCategoryGenerator.GetCategory();
        var updatedCategory = BogusCategoryGenerator.GetCategory();

        var createCategoryRequest = new
        {
            originalCategory.Name
        };

        var createResponse = await _client.PostAsync($"api/v1/Categories", new StringContent(JsonConvert.SerializeObject(createCategoryRequest), Encoding.UTF8,
                        "application/json"));
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var createJson = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonConvert.DeserializeObject<CreateCategoryCommandResponse>(createJson);
        var categoryId = createResult!.Id;

        var updateCategoryRequest = new
        {
            Id = categoryId,
            updatedCategory.Name
        };

        // Act
        var updateResponse = await _client.PutAsync($"api/v1/Categories", new StringContent(JsonConvert.SerializeObject(updateCategoryRequest), Encoding.UTF8,
                        "application/json"));

        // Assert
        updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var updateJson = await updateResponse.Content.ReadAsStringAsync();
        var updateResult = JsonConvert.DeserializeObject<UpdateCategoryCommandResponse>(updateJson);

        updateResult!.Id.Should().Be(categoryId);
    }

    [Fact]
    public async Task GetById_ShouldReturnCategorySuccessfully()
    {
        // Arrange
        var category = BogusCategoryGenerator.GetCategory();

        var createCategoryRequest = new
        {
            category.Name
        };

        var createResponse = await _client.PostAsync($"api/v1/Categories", new StringContent(JsonConvert.SerializeObject(createCategoryRequest), Encoding.UTF8,
                        "application/json"));
        createResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var createJson = await createResponse.Content.ReadAsStringAsync();
        var createResult = JsonConvert.DeserializeObject<CreateCategoryCommandResponse>(createJson);
        var categoryId = createResult!.Id;

        // Act
        var getResponse = await _client.GetAsync($"api/v1/Categories/{categoryId}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getJson = await getResponse.Content.ReadAsStringAsync();
        var getResult = JsonConvert.DeserializeObject<GetCategoryByIdQueryResponse>(getJson);

        getResult!.Id.Should().Be(categoryId);
        getResult.Name.Should().Be(category.Name);
    }

    [Fact]
    public async Task GetAll_ShouldReturnPaginatedCategories()
    {
        // Arrange
        var category1 = BogusCategoryGenerator.GetCategory();
        var category2 = BogusCategoryGenerator.GetCategory();

        // Create first category
        var createCategory1Request = new { category1.Name };
        var createResponse1 = await _client.PostAsync($"api/v1/Categories", new StringContent(JsonConvert.SerializeObject(createCategory1Request), Encoding.UTF8,
                        "application/json"));
        createResponse1.StatusCode.Should().Be(HttpStatusCode.OK);

        // Create second category
        var createCategory2Request = new { category2.Name };
        var createResponse2 = await _client.PostAsync($"api/v1/Categories", new StringContent(JsonConvert.SerializeObject(createCategory2Request), Encoding.UTF8,
                        "application/json"));
        createResponse2.StatusCode.Should().Be(HttpStatusCode.OK);

        // Act
        var getAllResponse = await _client.GetAsync($"api/v1/Categories?pageNumber=1&recordsPerPage=10");

        // Assert
        getAllResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var getAllJson = await getAllResponse.Content.ReadAsStringAsync();
        var getAllResult = JsonConvert.DeserializeObject<GetCategoriesQueryResponse>(getAllJson);

        getAllResult!.Result.Should().NotBeNull();
        getAllResult.Result.TotalRecords.Should().BeGreaterThanOrEqualTo(2);
        getAllResult.Result.Results.Should().NotBeNull();
        getAllResult.Result.Results.Count.Should().BeGreaterThanOrEqualTo(2);
        getAllResult.Result.PageNumber.Should().Be(1);
        getAllResult.Result.RecordsPerPage.Should().Be(10);
    }

}
