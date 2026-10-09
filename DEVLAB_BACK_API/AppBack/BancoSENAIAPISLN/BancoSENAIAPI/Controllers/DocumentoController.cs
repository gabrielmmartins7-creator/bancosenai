using Microsoft.AspNetCore.Mvc;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class DocumentoController : ControllerBase
    {

        private readonly string _caminhoRaiz = Path.Combine(Directory.GetCurrentDirectory(), "ClienteArquivos");
        private static List<Models.DocumentoMetadados> _documentosMetadados = new List<Models.DocumentoMetadados>();
        private static int _nextId = 1;

        [HttpPost("upload/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(int codigoCliente, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Nenhum arquivo foi enviado.");

            string pastaCliente = Path.Combine(_caminhoRaiz, codigoCliente.ToString());

            if (!Directory.Exists(pastaCliente))
                Directory.CreateDirectory(pastaCliente);

            string extensao = Path.GetExtension(arquivo.FileName);
            string nomeOriginal = Path.GetFileNameWithoutExtension(arquivo.FileName);
            string novoNome = $"{codigoCliente}_{nomeOriginal}_{Guid.NewGuid()}{extensao}";

            string caminhoFinal = Path.Combine(pastaCliente, novoNome);

            using (var stream = new FileStream(caminhoFinal, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            var documentoMetadados = new Models.DocumentoMetadados
            {
                Id = _nextId++,
                Nome = nomeOriginal,
                Extensao = extensao,
                Caminho = caminhoFinal,
                CodigoCliente = codigoCliente
            };

            _documentosMetadados.Add(documentoMetadados);

            return Ok(new
            {
                mensagem = "Documento anexado com sucesso!",
                arquivoSalvo = novoNome
            });
        }

        [HttpGet("listar/{codigoCliente}")]
        public IActionResult ListarDocumentos(int codigoCliente)
        {
            var documentos = _documentosMetadados.Where(d => d.CodigoCliente == codigoCliente).ToList();

            if (!documentos.Any())
                return NotFound(new { message = "Nenhum documento encontrado para este cliente." });

            return Ok(documentos);
        }

        [HttpGet("download/{id}")]
        public IActionResult DownloadDocumento(int id)
        {
            var documento = _documentosMetadados.FirstOrDefault(d => d.Id == id);

            if (documento == null)
                return NotFound(new { message = "Documento não encontrado." });

            var caminhoArquivo = documento.Caminho;
            var nomeArquivo = documento.Nome + documento.Extensao;

            var fileBytes = System.IO.File.ReadAllBytes(caminhoArquivo);
            return File(fileBytes, "application/octet-stream", nomeArquivo);
        }

        [HttpDelete("excluir/{id}")]
        public IActionResult ExcluirDocumento(int id)
        {
            var documento = _documentosMetadados.FirstOrDefault(d => d.Id == id);

            if (documento == null)
                return NotFound(new { message = "Documento não encontrado." });

            _documentosMetadados.Remove(documento);
            System.IO.File.Delete(documento.Caminho);

            return Ok(new { message = "Documento excluído com sucesso." });
        }
    }
}
