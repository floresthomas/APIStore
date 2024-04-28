using API.StoreData;
using API.StoreShared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Store.Services
{
    public class ProductService : ICrudService<Product>
    {
        private readonly ApiStoreDbContext _context;
        public ProductService(ApiStoreDbContext context) 
        {
            _context = context;        
        }
        public async Task<IActionResult> Create(Product entity)
        {
            await _context.Products.AddAsync(entity);
            await _context.SaveChangesAsync();
            return new CreatedAtActionResult("GetById", "Products", new { id = entity.Id }, entity);
        }

        public async Task<IActionResult> Delete(Product entity)
        {
            if (entity == null) return new NotFoundResult();

            _context.Products.Remove(entity);
            await _context.SaveChangesAsync();
            return new NoContentResult();
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _context.Products.ToListAsync();
        }

        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return new NotFoundResult();
            return new OkObjectResult(product); 
        }

        public async Task<IActionResult> Update(Product entity)
        {
            _context.Products.Update(entity);
            await _context.SaveChangesAsync();
            return new NoContentResult();
        }
    }
}
