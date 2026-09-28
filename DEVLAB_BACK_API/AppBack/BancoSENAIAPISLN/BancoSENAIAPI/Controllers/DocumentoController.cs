using BancoSENAIAPI.Data;
using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentoController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly string _caminhoRaiz = Path.Combine(
            Directory.GetCurrentDirectory(), "ClienteArquivos"
        );

        private const long tamanhoMaximo = 2 * 1024 * 1024;

        private readonly string[] _extensoesPermitidas =
            { ".pdf", ".jpg", ".png" };

        public DocumentoController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("upload/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(
            int codigoCliente,
            IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest(new
                {
                    erro = "Nenhum arquivo foi enviado."
                });
            }

            if (arquivo.Length > tamanhoMaximo)
            {
                return BadRequest(new
                {
                    erro = "Regra R06F Violada: O arquivo excede o tamanho máximo permitido de 2 MB."
                });
            }

            string extensao =
                Path.GetExtension(arquivo.FileName).ToLowerInvariant();

            if (!_extensoesPermitidas.Contains(extensao))
            {
                return BadRequest(new
                {
                    erro = $"Regra R06G Violada: Extensão '{extensao}' inválida. " +
                           $"Extensões permitidas: {string.Join(", ", _extensoesPermitidas)}."
                });
            }

            string pastaCliente = Path.Combine(
                _caminhoRaiz,
                codigoCliente.ToString()
            );

            if (!Directory.Exists(pastaCliente))
            {
                Directory.CreateDirectory(pastaCliente);
            }

            string nomeOriginal =
                Path.GetFileNameWithoutExtension(arquivo.FileName);

            string novoNome =
                $"{codigoCliente}_{nomeOriginal}_{Guid.NewGuid()}{extensao}";

            string caminhoFinal =
                Path.Combine(pastaCliente, novoNome);

            using (var stream = new FileStream(
                caminhoFinal,
                FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var documento = new DocumentoMetadado
            {
                Name = nomeOriginal,
                Extensao = extensao,
                Caminho = caminhoFinal,
                CodigoCliente = codigoCliente
            };

            await _context.Documentos.AddAsync(documento);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem = "Documento anexado com sucesso",
                arquivoSalvo = novoNome
            });
        }

        [HttpGet("listar/{codigoCliente}")]
        public async Task<IActionResult> ListarArquivo(int codigoCliente)
        {
            var documentos = await _context.Documentos
                .Where(d => d.CodigoCliente == codigoCliente)
                .ToListAsync();

            if (!documentos.Any())
            {
                return NotFound(new
                {
                    mensagem = $"Nenhum documento encontrado para o cliente {codigoCliente}."
                });
            }

            return Ok(documentos);
        }

        [HttpGet("download/{id}")]
        public async Task<IActionResult> Download(int id)
        {
            var documento = await _context.Documentos
                .FirstOrDefaultAsync(d => d.Id == id);

            if (documento == null)
            {
                return NotFound("Documento não encontrado.");
            }

            if (!System.IO.File.Exists(documento.Caminho))
            {
                return NotFound(
                    "Arquivo físico não foi encontrado no servidor."
                );
            }

            byte[] fileBytes =
                await System.IO.File.ReadAllBytesAsync(documento.Caminho);

            string nomeArquivo =
                documento.Name + documento.Extensao;

            return File(
                fileBytes,
                "application/octet-stream",
                nomeArquivo
            );
        }

        [HttpDelete("excluir/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var documento = await _context.Documentos
                .FirstOrDefaultAsync(d => d.Id == id);

            if (documento == null)
            {
                return NotFound("Documento não encontrado.");
            }

            if (System.IO.File.Exists(documento.Caminho))
            {
                System.IO.File.Delete(documento.Caminho);
            }

            _context.Documentos.Remove(documento);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                mensagem =
                    "Documento e arquivo físico excluídos com sucesso."
            });
        }
    }
}