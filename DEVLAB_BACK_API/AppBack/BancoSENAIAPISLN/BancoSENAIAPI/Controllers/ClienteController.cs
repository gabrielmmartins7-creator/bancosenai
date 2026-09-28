using BancoSENAIAPI.Data;
using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClienteController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodos()
        {
            var clientes = await _context.Clientes.ToListAsync();

            return Ok(clientes);
        }

        [HttpGet("{codigo}")]
        public async Task<IActionResult> BuscarPorCodigo(int codigo)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.CodigoCliente == codigo);

            if (cliente == null)
            {
                return NotFound(new
                {
                    message = "Cliente não encontrado."
                });
            }

            return Ok(cliente);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Cliente cliente)
        {
            var existe = await _context.Clientes
                .AnyAsync(c => c.CodigoCliente == cliente.CodigoCliente);

            if (existe)
            {
                return BadRequest(new
                {
                    message = "Este código de cliente já existe."
                });
            }

            await _context.Clientes.AddAsync(cliente);

            await _context.SaveChangesAsync();

            return Created("", cliente);
        }

        [HttpPut("{codigo}")]
        public async Task<IActionResult> Atualizar(
            int codigo,
            [FromBody] Cliente clienteAtualizado)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.CodigoCliente == codigo);

            if (cliente == null)
            {
                return NotFound(new
                {
                    message = "Cliente não encontrado."
                });
            }

            cliente.NomeCliente = clienteAtualizado.NomeCliente;
            cliente.CPF = clienteAtualizado.CPF;
            cliente.NumeroAgencia = clienteAtualizado.NumeroAgencia;
            cliente.SaldoTotal = clienteAtualizado.SaldoTotal;
            cliente.DataNascimento = clienteAtualizado.DataNascimento;
            cliente.Sexo = clienteAtualizado.Sexo;
            cliente.Endereco = clienteAtualizado.Endereco;
            cliente.Cidade = clienteAtualizado.Cidade;
            cliente.Estado = clienteAtualizado.Estado;

            await _context.SaveChangesAsync();

            return Ok(cliente);
        }

        [HttpDelete("{codigo}")]
        public async Task<IActionResult> Apagar(int codigo)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.CodigoCliente == codigo);

            if (cliente == null)
            {
                return NotFound(new
                {
                    message = "Cliente não encontrado."
                });
            }

            _context.Clientes.Remove(cliente);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cliente apagado com sucesso."
            });
        }
    }
}