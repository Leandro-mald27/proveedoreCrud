using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using proveedoresCrud.config;
using proveedoresCrud.models;
using Microsoft.Extensions.Options;

namespace ProveedoresCrud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;

        /// <summary>
        /// Constructor del controlador de autenticación.
        /// </summary>
        /// <param name="jwtSettings">Configuración de JWT inyectada mediante IOptions.</param>
        public AuthController(IOptions<JwtSettings> jwtSettings)
        {
            _jwtSettings = jwtSettings.Value;
        }

        /// <summary>
        /// Inicia sesión con credenciales quemadas.
        /// </summary>
        /// <remarks>
        /// Ejemplo de petición:
        /// 
        ///     POST /api/Auth/login
        ///     {
        ///         "username": "admin",
        ///         "password": "password"
        ///     }
        /// 
        /// </remarks>
        /// <param name="user">Objeto que contiene las credenciales de inicio de sesión.</param>
        /// <returns>Token JWT si las credenciales son correctas, o mensaje de error si son incorrectas.</returns>
        /// <response code="200">Retorna el token JWT si el inicio de sesión es exitoso.</response>
        /// <response code="401">Retorna un mensaje de error si el usuario o contraseña son incorrectos.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<string>), 200)]
        [ProducesResponseType(typeof(ApiResponse<string>), 401)]
        public IActionResult Login([FromBody] userLoginDto user)
        {
            if (user.Username == "admin" && user.Password == "password")
            {
                var token = GenerateJwtToken(user.Username);
                return Ok(new ApiResponse<string>(token, "Login exitoso"));
            }
            return Unauthorized(new ApiResponse<string>("Usuario o contraseña incorrectos"));
        }

        /// <summary>
        /// Genera un token JWT para el usuario autenticado.
        /// </summary>
        /// <param name="username">Nombre de usuario para el token JWT.</param>
        /// <returns>Cadena con el token JWT generado.</returns>
        private string GenerateJwtToken(string username)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
