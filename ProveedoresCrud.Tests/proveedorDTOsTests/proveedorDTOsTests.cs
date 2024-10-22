using NUnit.Framework;
using proveedoresCrud.models;

namespace ProveedoresCrud.Tests.proveedorDTOsTests
{
    [TestFixture]
    public class ProveedorDTOsTests
    {
        [Test]
        public void ProveedorDTOs_Should_HaveValidData()
        {
            var proveedor = new ProveedorDTOs
            {
                Nit = "123456789",
                RazonSocial = "Proveedor SA",
                Direccion = "Calle 123 #45-67",
                Ciudad = "Bogotá",
                Departamento = "Cundinamarca",
                Correo = "contacto@proveedorsa.com",
                Activo = true,
                FechaCreacion = DateTime.Now,
                NombreContacto = "Juan Pérez",
                CorreoContacto = "juan.perez@proveedorsa.com"
            };

            Assert.That(proveedor.Nit, Is.EqualTo("123456789"));
            Assert.That(proveedor.RazonSocial, Is.EqualTo("Proveedor SA"));
            Assert.That(proveedor.Activo, Is.True);
        }
    }
}
