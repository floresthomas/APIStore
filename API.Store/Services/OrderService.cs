using API.StoreData;
using API.StoreShared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Store.Services
{
    public class OrderService : ICrudService<Order>
    {
        private readonly ApiStoreDbContext _context;

        public OrderService(ApiStoreDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Create(Order entity)
        {
            await _context.Orders.AddAsync(entity);
            await _context.OrderDetails.AddRangeAsync(entity.OrderDetails);
            await _context.SaveChangesAsync();

            return new CreatedAtActionResult("GetById", "Orders", new { id = entity.Id }, entity);
        }

        public async Task<IActionResult> Delete(Order entity)
        {
            var existingOrder = await _context.Orders.Include(od => od.OrderDetails).FirstOrDefaultAsync(o => o.Id == entity.Id);
            if (existingOrder == null) return new NotFoundResult();

            _context.OrderDetails.RemoveRange(existingOrder.OrderDetails);
            _context.Orders.Remove(existingOrder);

            await _context.SaveChangesAsync();
            return new NoContentResult();
        }

        public async Task<IEnumerable<Order>> GetAll()
        {
            return await _context.Orders.Include(od => od.OrderDetails).ToListAsync();
        }

        public async Task<IActionResult> GetById(int id)
        {
            var order = await _context.Orders.Include(od => od.OrderDetails).FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return new NotFoundResult();
            return new OkObjectResult(order);
        }

        public async Task<IActionResult> Update(Order entity)
        {
            var existingOrder = await _context.Orders.Include(od => od.OrderDetails).FirstOrDefaultAsync(o => o.Id == entity.Id);
            if (existingOrder != null) return new NotFoundResult();

            existingOrder.OrderNumber = entity.OrderNumber;
            existingOrder.OrderDate = entity.OrderDate;
            existingOrder.DeliveryDate = entity.DeliveryDate;
            existingOrder.ClientId = entity.ClientId;
            _context.OrderDetails.RemoveRange(existingOrder.OrderDetails);

            _context.Orders.Update(existingOrder);
            _context.OrderDetails.AddRangeAsync(existingOrder.OrderDetails);

            await _context.SaveChangesAsync();
            return new NoContentResult();
        }
    }
}
