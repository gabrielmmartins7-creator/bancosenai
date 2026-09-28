using BancoSENAIAPI.Data;
using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CarteiraController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CarteiraController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Carteira/1
        [HttpGet("{numeroCarteira}")]
        public async Task<IActionResult> Get(int numeroCarteira)
        {
            var carteira = await _context.Carteiras
                .FirstOrDefaultAsync(c => c.NumeroCarteira == numeroCarteira);

            if (carteira == null)
            {
                return NotFound("Carteira não encontrada.");
            }

            return Ok(carteira);
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodas()
        {
            var carteiras = await _context.Carteiras.ToListAsync();

            return Ok(carteiras);
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] Carteira novaCarteira)
        {
            var existe = await _context.Carteiras
                .AnyAsync(c => c.NumeroCarteira == novaCarteira.NumeroCarteira);

            if (existe)
            {
                return BadRequest(new
                {
                    message = "Este número de carteira já existe."
                });
            }

            if (novaCarteira.ApetiteCarteira < 0)
            {
                return BadRequest(new
                {
                    message = "O apetite da carteira não pode ser negativo."
                });
            }

            await _context.Carteiras.AddAsync(novaCarteira);

            await _context.SaveChangesAsync();

            return Created("", novaCarteira);
        }

        [HttpPut("{numero}")]
        public async Task<IActionResult> Atualizar(
            int numero,
            [FromBody] Carteira carteiraAtualizada)
        {
            var carteira = await _context.Carteiras
                .FirstOrDefaultAsync(c => c.NumeroCarteira == numero);

            if (carteira == null)
            {
                return NotFound(new
                {
                    message = "Carteira não encontrada."
                });
            }

            if (carteiraAtualizada.ApetiteCarteira < 0)
            {
                return BadRequest(new
                {
                    message = "O apetite da carteira não pode ser negativo."
                });
            }

            carteira.NomeCarteira = carteiraAtualizada.NomeCarteira;
            carteira.ApetiteCarteira = carteiraAtualizada.ApetiteCarteira;

            await _context.SaveChangesAsync();

            return Ok(carteira);
        }

        [HttpDelete("{numero}")]
        public async Task<IActionResult> Apagar(int numero)
        {
            var carteira = await _context.Carteiras
                .FirstOrDefaultAsync(c => c.NumeroCarteira == numero);

            if (carteira == null)
            {
                return NotFound(new
                {
                    message = "Carteira não encontrada."
                });
            }

            _context.Carteiras.Remove(carteira);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Carteira apagada com sucesso."
            });
        }
    }
}