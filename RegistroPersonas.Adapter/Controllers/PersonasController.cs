
using Microsoft.AspNetCore.Mvc;
using RegistroPersonas.Negocio;
using RegistroUsuarios.Domain;

namespace RegistroPersonas.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonasController : ControllerBase
    {
        private readonly PersonaService _service;

        public PersonasController(PersonaService service)
        {
            _service = service;
        }

        [HttpPost]
        public IActionResult Registrar([FromBody] Persona persona)
        {
            try
            {
                _service.RegistrarPersona(persona);
                return Ok(new { mensaje = "Persona registrada exitosamente." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Error interno: " + ex.Message });
            }
        }
    }
}

