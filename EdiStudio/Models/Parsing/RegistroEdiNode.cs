namespace EdiStudio.Models.Parsing
{
    /// <summary>
    /// Representa uma ocorrência concreta de registro reconhecida em uma
    /// linha de documento EDI.
    /// </summary>
    /// <remarks>
    /// Mantém a definição do tipo de registro, os valores extraídos e as
    /// relações hierárquicas de pai e filhos.
    /// </remarks>
    public class RegistroEdiNode
    {
        public int NumeroLinha { get; set; }
        public string LinhaOriginal { get; set; } = string.Empty;
        public TipoRegistro TipoRegistro { get; set; } = new();
        public RegistroEdiNode? Pai {  get; set; }
        public List<RegistroEdiNode> Filhos {  get; set; } = [];
        public List<CampoEdiValor> Campos { get; set; } = [];
        public bool FoiAlterado => Campos.Any(campo => campo.FoiAlterado);
    }
}
