using API.Store.Services;
using API.StoreShared;
using Microsoft.AspNetCore.Mvc;

namespace API.Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoriesController : ControllerBase
    {
        private readonly ICrudService<ProductCategory> _productCategoryService;

        public ProductCategoriesController(ICrudService<ProductCategory> productCategoryService)
        {
            _productCategoryService = productCategoryService;
        }

        [HttpGet]
        public async Task<IEnumerable<ProductCategory>> GetAll()
        {
            return await _productCategoryService.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return await _productCategoryService.GetById(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCategory entity)
        {
            return await _productCategoryService.Create(entity);
        }

        [HttpPut]
        public async Task<IActionResult> Update(ProductCategory entity)
        {
            return await _productCategoryService.Update(entity);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(ProductCategory entity)
        {
            return await _productCategoryService.Delete(entity);
        }
    }

}
