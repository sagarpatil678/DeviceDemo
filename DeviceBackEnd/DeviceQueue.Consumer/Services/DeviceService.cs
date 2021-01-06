using DeviceQueue.Consumer.Model;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceQueue.Consumer.Services
{
    public interface IDeviceService
    {
        public Task<List<Device>> Get();
        Task Update(Device device);

    }
    public class DeviceService : IDeviceService
    {
        private readonly IMongoCollection<Device> _devices;

        public DeviceService(IConfiguration configuration)
        {
            // Connects to MongoDB.
            var client = new MongoClient(configuration.GetConnectionString("demodb"));
            // Gets the Demo db.
            var database = client.GetDatabase("demo");
            //Fetches the device collection.
            _devices = database.GetCollection<Device>("device");
        }

        public async Task<List<Device>> Get()
        {
            return await _devices.Find(s => true).ToListAsync();
        }
        public async Task Update(Device device)
        {
            await _devices.ReplaceOneAsync(_ => _.Id == device.Id, device);
        }
    }
}
