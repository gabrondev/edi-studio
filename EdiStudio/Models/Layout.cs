using EdiStudio.Enums;

namespace EdiStudio.Models
{
    internal class Layout
    {
        public string Nome {  get; set; } = string.Empty;
        public TipoEdi TipoEdi { get; set; }
        public List<TipoRegistro> TiposRegistro { get; set; } = [];
    }
}
