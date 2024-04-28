using Microsoft.AspNetCore.Mvc;

namespace API.Store.Services
{
    public interface ICrudService<T>
    {
        Task<IEnumerable<T>> GetAll();
        Task<IActionResult> GetById(int id);
        Task<IActionResult> Create(T entity);
        Task<IActionResult> Update(T entity);
        Task<IActionResult> Delete(T entity);
    }
}
