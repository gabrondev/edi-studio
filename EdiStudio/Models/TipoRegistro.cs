namespace EdiStudio.Models
{
    internal class TipoRegistro
    {
        public int Identificador {  get; set; }
        public int? IdentificadorRegistroPai { get; set; }
        public string Nome { get; set; } = string.Empty;
        public int QtdeMaxOcorrenciasPorRegistroPai { get; set; }
        public int TamanhoTipoRegistro {  get; set; }
        public List<Campo> Campos { get; set; } = [];
    }
}
