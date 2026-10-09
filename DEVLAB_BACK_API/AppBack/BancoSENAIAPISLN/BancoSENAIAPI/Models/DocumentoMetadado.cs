using System.ComponentModel.DataAnnotations;

namespace BancoSENAIAPI.Models
{
    public class DocumentoMetadado
    {
        [Key]

        public int Id {  get; set; }
        public string Name { get; set;}
        public string Extensao { get; set;}
        public string Caminho { get; set;}
        public int CodigoCliente { get; set;}
    }
}
