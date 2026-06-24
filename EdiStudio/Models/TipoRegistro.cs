namespace EdiStudio.Models
{
    public class TipoRegistro
    {
        public string Identificador { get; set; } = string.Empty;
        public string? IdentificadorRegistroPai { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int QtdeMaxOcorrenciasPorRegistroPai { get; set; }
        public int TamanhoTipoRegistro {  get; set; }
        public List<Campo> Campos { get; set; } = [];
    }
}
