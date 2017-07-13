using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewOrbit.Messaging;
using SampleDomain.Messages;
using Web.Models;

namespace Web.Controllers
{
    [Route("api/order")]
    public class OrderController : Controller
    {
        private readonly IClientCommandBus commandBus;

        public OrderController(IClientCommandBus commandBus)
        {
            this.commandBus = commandBus;
        }

        [HttpPost]
        public async Task Post([FromBody] OrderDto data)
        {
            var cmd = new CreateOrder
            {
                CorrelationId = Guid.NewGuid().ToString(),
                Id = Guid.NewGuid().ToString(),
                CustomerCode = data.Customer,
                ReferenceCode = data.Reference
            };
            await this.commandBus.Submit(cmd);
        }
    }
}
