using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Publisher_Api.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Publisher_Api.Data
{
    public interface IDeviceContext
    {
        IMongoDatabase deviceDb { get; }
    }
    public class DeviceContext : IDeviceContext
    {
        public IMongoDatabase deviceDb { get; set; }

        public DeviceContext(IConfiguration configuration)
        {
            // Connects to MongoDB.
            var client = new MongoClient(configuration.GetConnectionString("demodb"));
            // Gets the Demo db.
            deviceDb = client.GetDatabase(configuration["ConnectionStrings:database"]);

        }

    }
}
