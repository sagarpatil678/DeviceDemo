using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Publisher_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Publisher_Api.Data
{
    public interface IDeviceRepo
    {
        Task<IList<Device>> GetAll();
        Task Update(Device device);
    }
    public class DeviceRepo : IDeviceRepo
    {
        private readonly IMongoCollection<Device> _devices;
        public DeviceRepo(IConfiguration configuration, IDeviceContext deviceContext)
        {
            _devices = deviceContext.deviceDb.GetCollection<Device>(configuration["ConnectionStrings:collection"]);
        }

        public async Task<IList<Device>> GetAll()
        {
            return await _devices.Find(_ => true).ToListAsync();
        }

        public async Task Update(Device device)
        {
            await _devices.ReplaceOneAsync(_ => _.Id == device.Id, device);
        }
    }
}
