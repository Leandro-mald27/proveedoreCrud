using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using proveedoresCrud.models;
using ProveedoresCrud.Services;

namespace ProveedoresCrud.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // Se requiere autenticación para todas las rutas del controlador
    public class ProveedoresController : ControllerBase
    {
        private readonly IProveedorService _proveedorService;

        public ProveedoresController(IProveedorService proveedorService)
        {
            _proveedorService = proveedorService;
        }

        /// <summary>
        /// Obtiene la lista de todos los proveedores.
        /// </summary>
        /// <returns>Lista de proveedores.</returns>
        /// <response code="200">Devuelve la lista de proveedores</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProveedores()
        {
            try
            {
                var proveedores = await _proveedorService.GetAllAsync();
                return Ok(new ApiResponse<IEnumerable<Proveedor>>(proveedores));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"Error interno: {ex.Message}", success: false));
            }
        }

        /// <summary>
        /// Obtiene un objeto por su NIT.
        /// </summary>
        /// <param name="nit">NIT del objeto.</param>
        /// <response code="200">OK. Devuelve el objeto solicitado.</response>        
        /// <response code="404">NotFound. No se ha encontrado el objeto solicitado.</response>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>
        [HttpGet("{nit}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetProveedor(string nit)
        {
            try
            {
                if (string.IsNullOrEmpty(nit))
                    return BadRequest(new ApiResponse<string>("El NIT no puede estar vacío", success: false));

                var proveedor = await _proveedorService.GetByNitAsync(nit);
                if (proveedor == null)
                    return NotFound(new ApiResponse<string>($"No se encontró el proveedor con NIT: {nit}", success: false));

                return Ok(new ApiResponse<Proveedor>(proveedor));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"Error interno: {ex.Message}", success: false));
            }
        }

        /// <summary>
        /// Crea un nuevo proveedor.
        /// </summary>
        /// <param name="proveedorDto">El objeto proveedor a crear.</param>
        /// <returns>El proveedor recién creado.</returns>
        /// <response code="201">Devuelve el proveedor recién creado</response>
        /// <response code="400">Si el proveedor es nulo o inválido</response>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>  
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateProveedor([FromBody] ProveedorDTOs proveedorDto)
        {
            try
            {
                if (proveedorDto == null)
                    return BadRequest(new ApiResponse<string>("El proveedor no puede ser nulo", success: false));

                if (string.IsNullOrEmpty(proveedorDto.Nit))
                    return BadRequest(new ApiResponse<string>("El NIT del proveedor es obligatorio", success: false));

                var nuevoProveedor = new Proveedor
                {
                    Nit = proveedorDto.Nit,
                    RazonSocial = proveedorDto.RazonSocial,
                    Direccion = proveedorDto.Direccion,
                    Ciudad = proveedorDto.Ciudad,
                    Departamento = proveedorDto.Departamento,
                    Correo = proveedorDto.Correo,
                    Activo = proveedorDto.Activo,
                    FechaCreacion = proveedorDto.FechaCreacion,
                    NombreContacto = proveedorDto.NombreContacto,
                    CorreoContacto = proveedorDto.CorreoContacto
                };

                await _proveedorService.AddAsync(nuevoProveedor);
                return CreatedAtAction(nameof(GetProveedor), new { nit = nuevoProveedor.Nit }, new ApiResponse<Proveedor>(nuevoProveedor, "Proveedor creado con éxito"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"Error interno: {ex.Message}", success: false));
            }
        }

        /// <summary>
        /// Actualiza un proveedor existente por su NIT.
        /// </summary>
        /// <param name="nit">NIT del proveedor a actualizar.</param>
        /// <param name="proveedorDto">El objeto proveedor actualizado.</param>
        /// <returns>Un mensaje de éxito si la actualización es exitosa.</returns>
        /// <response code="200">Si la actualización es exitosa</response>
        /// <response code="400">Si el NIT no coincide</response>
        /// <response code="404">Si no se encuentra el proveedor</response>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>
        [HttpPut("{nit}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateProveedor(string nit, [FromBody] ProveedorDTOs proveedorDto)
        {
            try
            {
                if (string.IsNullOrEmpty(nit) || nit != proveedorDto.Nit)
                    return BadRequest(new ApiResponse<string>("El NIT no coincide", success: false));

                var proveedorExistente = await _proveedorService.GetByNitAsync(nit);
                if (proveedorExistente == null)
                    return NotFound(new ApiResponse<string>($"No se encontró el proveedor con NIT: {nit}", success: false));

                // Actualizar solo los campos permitidos
                proveedorExistente.RazonSocial = proveedorDto.RazonSocial;
                proveedorExistente.Direccion = proveedorDto.Direccion;
                proveedorExistente.Ciudad = proveedorDto.Ciudad;
                proveedorExistente.Departamento = proveedorDto.Departamento;
                proveedorExistente.Correo = proveedorDto.Correo;
                proveedorExistente.Activo = proveedorDto.Activo;
                proveedorExistente.FechaCreacion = proveedorDto.FechaCreacion;
                proveedorExistente.NombreContacto = proveedorDto.NombreContacto;
                proveedorExistente.CorreoContacto = proveedorDto.CorreoContacto;

                await _proveedorService.UpdateAsync(proveedorExistente);
                return Ok(new ApiResponse<string>("Proveedor actualizado con éxito", success: true));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"Error interno: {ex.Message}", success: false));
            }
        }

        /// <summary>
        /// Elimina un proveedor por su NIT.
        /// </summary>
        /// <param name="nit">NIT del proveedor a eliminar.</param>
        /// <returns>Un mensaje de éxito si la eliminación es exitosa.</returns>
        /// <response code="200">Si la eliminación es exitosa</response>
        /// <response code="400">Si el NIT es inválido</response>
        /// <response code="404">Si no se encuentra el proveedor</response>
        /// <response code="401">Unauthorized. No se ha indicado o es incorrecto el Token JWT de acceso.</response>
        [HttpDelete("{nit}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteProveedor(string nit)
        {
            try
            {
                if (string.IsNullOrEmpty(nit))
                    return BadRequest(new ApiResponse<string>("El NIT no puede estar vacío", success: false));

                var proveedorExistente = await _proveedorService.GetByNitAsync(nit);
                if (proveedorExistente == null)
                    return NotFound(new ApiResponse<string>($"No se encontró el proveedor con NIT: {nit}", success: false));

                await _proveedorService.DeleteAsync(nit);
                return Ok(new ApiResponse<string>("Proveedor eliminado con éxito", success: true));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<string>($"Error interno: {ex.Message}", success: false));
            }
        }
    }
}
