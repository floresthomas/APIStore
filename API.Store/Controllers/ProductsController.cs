using API.Store.Services;
using API.StoreShared;
using Microsoft.AspNetCore.Mvc;

namespace API.Store.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ICrudService<Product> _productService;

        public ProductsController(ICrudService<Product> productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IEnumerable<Product>> GetAll()
        {
            return await _productService.GetAll();
        }
        [HttpGet("{id}")] 
        public async Task<IActionResult> GetById(int id)
        {
            return await _productService.GetById(id);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product entity)
        {
            return await _productService.Create(entity);
        }
        [HttpPut]
        public async Task<IActionResult> Update(Product entity)
        {
            return await _productService.Update(entity);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(Product entity)
        {
            return await _productService.Delete(entity);
        }
    }
}
