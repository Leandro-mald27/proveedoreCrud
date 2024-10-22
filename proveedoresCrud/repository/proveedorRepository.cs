using MongoDB.Driver;
using proveedoresCrud.models;

namespace ProveedoresCrud.Repositories
{
    public class ProveedorRepository : IProveedorRepository
    {
        private readonly IMongoCollection<Proveedor> _collection;

        public ProveedorRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<Proveedor>("Proveedores");
        }

        public async Task<Proveedor> GetByNitAsync(string nit)
        {
            return await _collection.Find(p => p.Nit == nit).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Proveedor>> GetAllAsync()
        {
            return await _collection.Find(p => true).ToListAsync();
        }

        public async Task AddAsync(Proveedor proveedor)
        {
            await _collection.InsertOneAsync(proveedor);
        }

        public async Task UpdateAsync(Proveedor proveedor)
        {
            await _collection.ReplaceOneAsync(p => p.Nit == proveedor.Nit, proveedor);
        }

        public async Task DeleteAsync(string nit)
        {
            await _collection.DeleteOneAsync(p => p.Nit == nit);
        }
    }
}
