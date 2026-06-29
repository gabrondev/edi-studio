using EdiStudio.Enums;

namespace EdiStudio.Models
{
    /// <summary>
    /// Define a estrutura utilizada para interpretar documentos de um
    /// determinado padrão EDI.
    /// </summary>
    public class Layout
    {
        public string Nome {  get; set; } = string.Empty;
        public TipoEdi TipoEdi { get; set; }
        public List<TipoRegistro> TiposRegistro { get; set; } = [];
    }
}
