using ProveedoresCrud.Repositories;
using proveedoresCrud.models;

namespace ProveedoresCrud.Services
{
    public class ProveedorService : IProveedorService
    {
        private readonly IProveedorRepository _proveedorRepository;

        public ProveedorService(IProveedorRepository proveedorRepository)
        {
            _proveedorRepository = proveedorRepository;
        }

        public async Task<IEnumerable<Proveedor>> GetAllAsync()
        {
            return await _proveedorRepository.GetAllAsync();
        }

        public async Task<Proveedor> GetByNitAsync(string nit)
        {
            return await _proveedorRepository.GetByNitAsync(nit);
        }

        public async Task AddAsync(Proveedor proveedor)
        {
            await _proveedorRepository.AddAsync(proveedor);
        }

        public async Task UpdateAsync(Proveedor proveedor)
        {
            await _proveedorRepository.UpdateAsync(proveedor);
        }

        public async Task DeleteAsync(string nit)
        {
            await _proveedorRepository.DeleteAsync(nit);
        }
    }
}
