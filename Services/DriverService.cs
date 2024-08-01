using Microsoft.Extensions.Options;
using MongoCrud.Configurations;
using MongoCrud.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoCrud.Services
{
    public class DriverService
    {
        private readonly IMongoCollection<Drivers> _drivers;

        public DriverService(IOptions<DatabseSettings> settings)
        {
            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            var mongoDbDatabase = mongoClient.GetDatabase(settings.Value.Database);
            _drivers = mongoDbDatabase.GetCollection<Drivers>(settings.Value.CollectionName);
        }

        public async Task<object> GetAsync()
        {
            return await _drivers.FindAsync(x => true);
        }
        public async Task<object> GetAsyncById (string Id)
        {
            var driver =  await _drivers.FindAsync(x => x.Id == Id);

            if(driver == null)
            {
                return null;
            }
            return driver;

        }
        public async Task InsertDriver(Drivers driver)
        {
             await _drivers.InsertOneAsync(driver);

        }
        public async Task UpdateDriver (Drivers Driver)
        {
            await _drivers.ReplaceOneAsync(x => x.Id == Driver.Id, Driver);
        }
        public async Task DeleteDriver(string Id)
        {
            await _drivers.DeleteOneAsync(x => x.Id == Id);
        }
    }
}
