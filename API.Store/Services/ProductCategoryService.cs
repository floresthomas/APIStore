using API.Store.Services.Interfaces;
using API.StoreData;
using API.StoreShared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Store.Services
{
    public class ProductCategoryService : ICrudService<ProductCategory>
    {
        private readonly ApiStoreDbContext _context;

        public ProductCategoryService(ApiStoreDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Create(ProductCategory entity)
        {
           await _context.ProductCategories.AddAsync(entity);
           await _context.SaveChangesAsync();
            return new CreatedAtActionResult("GetById", "ProductCategories", new { id = entity.Id }, entity);
        }

        public async Task<IActionResult> Delete(ProductCategory entity)
        {
            _context.ProductCategories.Remove(entity);
            await _context.SaveChangesAsync();
            return new NoContentResult();
        }

        public async Task<IEnumerable<ProductCategory>> GetAll()
        {
            return await _context.ProductCategories.ToListAsync();
        }

        public async Task<IActionResult> GetById(int id)
        {
            var productCategory = await _context.ProductCategories.FirstOrDefaultAsync(pc => pc.Id == id);
            if (productCategory == null) return new NotFoundResult();

            return new OkObjectResult(productCategory);
        }

        public async Task<IActionResult> Update(ProductCategory entity)
        {
            _context.ProductCategories.Update(entity);
            await _context.SaveChangesAsync();
            return new NoContentResult();
        }
    }
}
