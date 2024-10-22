using NUnit.Framework;
using Moq;
using ProveedoresCrud.Services;
using ProveedoresCrud.Repositories;
using proveedoresCrud.models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProveedoresCrud.Tests.ProveedorServiceTests
{
    [TestFixture]
    public class ProveedorServiceTests
    {
        private Mock<IProveedorRepository> _proveedorRepoMock;
        private ProveedorService _service;

        [SetUp]
        public void SetUp()
        {
            _proveedorRepoMock = new Mock<IProveedorRepository>();
            _service = new ProveedorService(_proveedorRepoMock.Object);
        }

        [Test]
        public async Task GetAllAsync_Should_ReturnListOfProveedores()
        {
            var proveedoresList = new List<Proveedor> { new Proveedor { Nit = "123" }, new Proveedor { Nit = "456" } };
            _proveedorRepoMock.Setup(repo => repo.GetAllAsync()).ReturnsAsync(proveedoresList);

            var result = await _service.GetAllAsync();

            Assert.That(result, Is.EqualTo(proveedoresList));
        }
    }
}
