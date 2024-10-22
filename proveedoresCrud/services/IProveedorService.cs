using proveedoresCrud.models;

namespace ProveedoresCrud.Services
{
    public interface IProveedorService
    {
        Task<IEnumerable<Proveedor>> GetAllAsync();
        Task<Proveedor> GetByNitAsync(string nit);
        Task AddAsync(Proveedor proveedor);
        Task UpdateAsync(Proveedor proveedor);
        Task DeleteAsync(string nit);
    }
}
