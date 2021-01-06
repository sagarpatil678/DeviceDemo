using MongoDB.Driver;
using System;

namespace Publisher_API.Data
{
    public interface IDeviceContext
    {
        public Task<List<Device>> Get();

    }
    public class DeviceContext : IDeviceContext
    {
        private readonly IMongoCollection<Device> _device;

        public DeviceService(IConfiguration configuration)
        {
            // Connects to MongoDB.
            var client = new MongoClient(configuration.GetConnectionString("demodb"));
            // Gets the Demo db.
            var database = client.GetDatabase("demo");
            //Fetches the device collection.
            _device = database.GetCollection<Device>("device");
        }

        public async Task<List<Device>> Get()
        {
            //Gets all supplements. 
            return await _device.Find(s => true).ToListAsync();
        }
    }
}
