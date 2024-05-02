using API.Store.Services.Interfaces;
using API.StoreShared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Store.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ICrudService<Order> _orderService;

        public OrdersController(ICrudService<Order> orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<IEnumerable<Order>> GetAll()
        {
            return await _orderService.GetAll();
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return await _orderService.GetById(id);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Order entity)
        {
            return await _orderService.Create(entity);
        }
        [HttpPut]
        public async Task<IActionResult> Update(Order entity)
        {
            return await _orderService.Update(entity);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(Order entity)
        {
            return await _orderService.Delete(entity);
        }
    }
}
