namespace EdiStudio.Models
{
    internal class Layout
    {
        public string Nome {  get; set; } = string.Empty;
        public List<TipoRegistro> TiposRegistro { get; set; } = [];
    }
}
