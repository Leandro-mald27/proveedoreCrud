using NUnit.Framework;
using Moq;
using Microsoft.Extensions.Options;
using ProveedoresCrud.Controllers;
using proveedoresCrud.config;
using proveedoresCrud.models;
using Microsoft.AspNetCore.Mvc;

namespace ProveedoresCrud.Tests.proveedoresControllerTests
{
    [TestFixture]
    public class AuthControllerTests
    {
        private AuthController _authController;
        private Mock<IOptions<JwtSettings>> _jwtSettingsMock;

        [SetUp]
        public void SetUp()
        {
            var jwtSettings = new JwtSettings
            {
                SecretKey = "your-test-secret-key",
                Issuer = "test-issuer",
                Audience = "test-audience",
                ExpirationInMinutes = 60
            };

            _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
            _jwtSettingsMock.Setup(s => s.Value).Returns(jwtSettings);

            _authController = new AuthController(_jwtSettingsMock.Object);
        }

        [Test]
        public void Login_WithValidCredentials_ShouldReturnToken()
        {
            var user = new userLoginDto { Username = "admin", Password = "password" };

            var result = _authController.Login(user) as OkObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(200));

            var response = result.Value as ApiResponse<string>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Data, Is.Not.Empty);
            Assert.That(response.Data.Length, Is.GreaterThan(20));
        }



        [Test]
        public void Login_WithInvalidCredentials_ShouldReturnUnauthorized()
        {

            var user = new userLoginDto { Username = "invalid", Password = "wrong" };

            var result = _authController.Login(user) as UnauthorizedObjectResult;

            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(401));

            var response = result.Value as ApiResponse<string>;
            Assert.That(response, Is.Not.Null);
            Assert.That(response.Message, Is.EqualTo("Usuario o contraseña incorrectos"));
        }

    }
}
