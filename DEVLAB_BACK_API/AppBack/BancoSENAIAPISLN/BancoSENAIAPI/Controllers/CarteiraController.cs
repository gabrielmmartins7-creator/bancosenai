using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CarteiraController : ControllerBase
    {
        private static List<Carteira> _carteiras = new List<Carteira>
        {
            new Carteira
            {
                NumeroCarteira = 1,
                NomeCarteira = "Agro",
                ApetiteCarteira = 1000000
            },

            new Carteira
            {
                NumeroCarteira = 2,
                NomeCarteira = "Varejo",
                ApetiteCarteira = 1500000
            },

            new Carteira
            {
                NumeroCarteira = 3,
                NomeCarteira = "Atacado",
                ApetiteCarteira = 2000000
            }
        };

        [HttpGet]
        public IActionResult ListarTodas()
        {
            return Ok(_carteiras);
        }

        [HttpPost]
        public IActionResult Cadastrar([FromBody] Carteira novaCarteira)
        {
            if (_carteiras.Any(c => c.NumeroCarteira == novaCarteira.NumeroCarteira))
            {
                return BadRequest(new { message = "Este número de carteira já existe." });
            }

            if (novaCarteira.ApetiteCarteira < 0)
            {
                return BadRequest(new { message = "O apetite da carteira não pode ser negativo." });
            }

            _carteiras.Add(novaCarteira);

            return Created("", novaCarteira);
        }
        [HttpPut("{numero}")]
        public IActionResult Atualizar(int numero, [FromBody] Carteira carteiraAtualizada)
        {
            var carteira = _carteiras.FirstOrDefault(c => c.NumeroCarteira == numero);

            if (carteira == null)
            {
                return NotFound(new { message = "Carteira não encontrada." });
            }

            if (carteiraAtualizada.ApetiteCarteira < 0)
            {
                return BadRequest(new { message = "O apetite da carteira não pode ser negativo." });
            }

            carteira.NomeCarteira = carteiraAtualizada.NomeCarteira;
            carteira.ApetiteCarteira = carteiraAtualizada.ApetiteCarteira;

            return Ok(carteira);
        }
        [HttpDelete("{numero}")]
        public IActionResult Apagar(int numero)
        {
            var carteira = _carteiras.FirstOrDefault(c => c.NumeroCarteira == numero);

            if (carteira == null)
            {
                return NotFound(new { message = "Carteira não encontrada." });
            }

            _carteiras.Remove(carteira);

            return Ok(new { message = "Carteira apagada com sucesso." });
        }








    }

}