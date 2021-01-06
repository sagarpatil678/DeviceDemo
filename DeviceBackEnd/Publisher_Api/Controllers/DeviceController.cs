using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Publisher_Api.Model;
using Publisher_Api.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Publisher_Api.Data;

namespace Publisher_Api.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private readonly IMessageQueueService _messageService;
        private readonly IDeviceRepo _deviceRepo;

        public DeviceController(IMessageQueueService messageService, IDeviceRepo deviceRepo)
        {
            _messageService = messageService;
            _deviceRepo = deviceRepo;
        }

        [HttpPost("AssignedDevice")]
        public bool Post([FromBody]Device payload)
        {
            string queueMessage = JsonConvert.SerializeObject(payload);
            return _messageService.Enqueue(queueMessage);
        }

        [HttpGet]
        [Route("All")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _deviceRepo.GetAll());
        }

        [HttpPut("UpdateDevice")]
        public async Task<IActionResult> Update(Device device)
        {
            await _deviceRepo.Update(device);
            return Ok(device);
        }

    }
}
