using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Obo.Objects;

namespace Obo.OrderAPI.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        [HttpGet("all")]
        [Authorize(Policy = "OnBehalfOf")]
        public IEnumerable<Order> GetOrders()
        {
            return
            [
                new Order { OrderId = 102, CustomerId = 1, TotalAmount = 75.00m },
                new Order { OrderId = 101, CustomerId = 1, TotalAmount = 250.00m },
                new Order { OrderId = 201, CustomerId = 2, TotalAmount = 200.00m },
                new Order { OrderId = 102, CustomerId = 2, TotalAmount = 150.50m },
                new Order { OrderId = 103, CustomerId = 3, TotalAmount = 325.75m },
                new Order { OrderId = 302, CustomerId = 3, TotalAmount = 200.00m }
            ];
        }
    }
}
