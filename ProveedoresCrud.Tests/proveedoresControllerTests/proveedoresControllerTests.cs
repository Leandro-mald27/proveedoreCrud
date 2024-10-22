using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using ProveedoresCrud.Controllers;
using ProveedoresCrud.Services;
using proveedoresCrud.models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ProveedoresCrud.Tests.ProveedoresControllerTests
{
    [TestFixture]
    public class ProveedoresControllerTests
    {
        private Mock<IProveedorService> _proveedorServiceMock;
        private ProveedoresController _controller;

        [SetUp]
        public void SetUp()
        {
            _proveedorServiceMock = new Mock<IProveedorService>();
            _controller = new ProveedoresController(_proveedorServiceMock.Object);
        }

        [Test]
        public async Task GetProveedores_Should_ReturnOkResult_WithListOfProveedores()
        {
            var proveedoresList = new List<Proveedor> { new Proveedor { Nit = "123" } };
            _proveedorServiceMock.Setup(service => service.GetAllAsync()).ReturnsAsync(proveedoresList);

            var result = await _controller.GetProveedores() as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));
        }
    }
}
