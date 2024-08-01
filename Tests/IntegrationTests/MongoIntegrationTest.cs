using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Mongo2Go;
using MongoDB.Driver;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using MongoCrud.Configurations;
using MongoCrud.Model;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Hosting;

public class DriverServiceTests : IClassFixture<WebApplicationFactory<Startup>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Startup> _factory;
    private readonly MongoDbRunner _mongoDbRunner;
    private readonly HttpClient _client;
    private readonly IMongoDatabase _database;
    private readonly string _collectionName = "Drivers";

    public DriverServiceTests(WebApplicationFactory<Startup> factory)
    {
        _factory = factory;

        // Start MongoDB instance
        _mongoDbRunner = MongoDbRunner.Start();
        var mongoClient = new MongoClient(_mongoDbRunner.ConnectionString);
        _database = mongoClient.GetDatabase("TestDatabase");

        // Create HttpClient
        _client = _factory.CreateClient();
    }

    public async Task InitializeAsync()
    { 
    }

    public async Task DisposeAsync()
    {
     
        _mongoDbRunner.Dispose();
    }

    [Fact]
    public async Task InsertDriver_ShouldAddDriverToDatabase()
    {
        // Arrange
        var driver = new Drivers
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test Driver"
        };

        var content = new StringContent(JsonConvert.SerializeObject(driver), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/drivers", content);
        response.EnsureSuccessStatusCode();

        // Assert
        var result = await _client.GetAsync($"/api/drivers/{driver.Id}");
        var responseBody = await result.Content.ReadAsStringAsync();
        var fetchedDriver = JsonConvert.DeserializeObject<Drivers>(responseBody);

        Assert.Equal(driver.Id, fetchedDriver.Id);
        Assert.Equal(driver.Name, fetchedDriver.Name);
    }

    [Fact]
    public async Task GetAllDrivers_ShouldReturnAllDrivers()
    {
        // Arrange
        var driver1 = new Drivers
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Driver 1"
        };

        var driver2 = new Drivers
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Driver 2"
        };

        await _database.GetCollection<Drivers>(_collectionName).InsertOneAsync(driver1);
        await _database.GetCollection<Drivers>(_collectionName).InsertOneAsync(driver2);

       
        var response = await _client.GetAsync("/api/drivers");
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        var drivers = JsonConvert.DeserializeObject<List<Drivers>>(responseBody);

      
        Assert.Contains(drivers, d => d.Id == driver1.Id);
        Assert.Contains(drivers, d => d.Id == driver2.Id);
    }

    [Fact]
    public async Task UpdateDriver_ShouldModifyExistingDriver()
    {
      
        var driver = new Drivers
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Original Name"
        };

        await _database.GetCollection<Drivers>(_collectionName).InsertOneAsync(driver);

        driver.Name = "Updated Name";
        var content = new StringContent(JsonConvert.SerializeObject(driver), Encoding.UTF8, "application/json");

        var response = await _client.PutAsync($"/api/drivers/{driver.Id}", content);
        response.EnsureSuccessStatusCode();

    
        var updatedResponse = await _client.GetAsync($"/api/drivers/{driver.Id}");
        var updatedResponseBody = await updatedResponse.Content.ReadAsStringAsync();
        var updatedDriver = JsonConvert.DeserializeObject<Drivers>(updatedResponseBody);

        Assert.Equal("Updated Name", updatedDriver.Name);
    }

    [Fact]
    public async Task DeleteDriver_ShouldRemoveDriver()
    {

        var driver = new Drivers
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Driver to Delete"
        };

        await _database.GetCollection<Drivers>(_collectionName).InsertOneAsync(driver);

  
        var response = await _client.DeleteAsync($"/api/drivers/{driver.Id}");
        response.EnsureSuccessStatusCode();

      
        var deletedResponse = await _client.GetAsync($"/api/drivers/{driver.Id}");
        Assert.Equal(System.Net.HttpStatusCode.NotFound, deletedResponse.StatusCode);
    }
}
