using proveedoresCrud.models;

namespace ProveedoresCrud.Repositories
{
    public interface IProveedorRepository
    {
        Task<Proveedor> GetByNitAsync(string nit);
        Task<IEnumerable<Proveedor>> GetAllAsync();
        Task AddAsync(Proveedor proveedor);
        Task UpdateAsync(Proveedor proveedor);
        Task DeleteAsync(string nit);
    }
}
