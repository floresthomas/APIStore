using API.Store.Services.Interfaces;
using API.StoreShared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Store.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ClientsController : ControllerBase
    {
        private readonly ICrudService<Client> _clientService;
        public ClientsController (ICrudService<Client> clientService)
        {
            _clientService = clientService;
        }

        [HttpGet]
        public async Task<IEnumerable<Client>> GetAll()
        {
            return await _clientService.GetAll();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            return await _clientService.GetById(id);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Client client)
        {
            return await _clientService.Create(client);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Client client)
        {
            return await _clientService.Update(client);
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(Client client)
        {
            return await _clientService.Delete(client);
        }
    }
}
