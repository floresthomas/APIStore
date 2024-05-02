using API.Store.Services.Interfaces;
using API.StoreData;
using API.StoreShared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Store.Services
{
    public class ClientService : ICrudService<Client>
    {
        private readonly ApiStoreDbContext _context;

        public ClientService(ApiStoreDbContext context) 
        {  
            _context = context; 
        }

        public async Task<IEnumerable<Client>> GetAll()
        {
            return await _context.Clients.ToListAsync();
        }

        public async Task<IActionResult> GetById(int id)
        {
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == id);
            if (client == null) return new NotFoundResult();
            return new OkObjectResult(client);
        }

        public async Task<IActionResult> Create(Client entity)
        {
            await _context.Clients.AddAsync(entity);
            await _context.SaveChangesAsync();
            return new CreatedAtActionResult("GetById", "Clients", new { id = entity.Id }, entity);
        }
        public async Task<IActionResult> Update(Client entity)
        {
            _context.Clients.Update(entity);
            await _context.SaveChangesAsync();
            return new NoContentResult();
        }
        public async Task<IActionResult> Delete(Client entity)
        {
            if (entity == null) return new NotFoundResult();
            _context.Clients.Remove(entity);
            await _context.SaveChangesAsync();
            return new NoContentResult();
        }
    }
}
