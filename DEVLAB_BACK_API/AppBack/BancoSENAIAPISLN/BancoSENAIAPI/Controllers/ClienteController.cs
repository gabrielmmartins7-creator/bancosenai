using BancoSENAIAPI.Models;
using BancoSENAIAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly ClienteService _service;

        public ClienteController()
        {
            _service = new ClienteService();
        }

        [HttpGet]
        public IActionResult ListarTodos()
        {
            return Ok(_service.ListarTodos());
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] Cliente cliente)
        {
            var novoCliente = _service.Cadastrar(cliente);

            return Created("", novoCliente);
        }

        [HttpGet("{codigo}")]
        public IActionResult BuscarPorCodigo(int codigo)
        {
            var cliente = _service.BuscarPorCodigo(codigo);

            if (cliente == null)
            {
                return NotFound(new { message = "Cliente não encontrado." });
            }

            return Ok(cliente);
        }
    }
}