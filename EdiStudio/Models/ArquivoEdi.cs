namespace EdiStudio.Models
{
    internal class ArquivoEdi
    {
        public Layout Layout { get; set; } = new();
        public List<Registro> Registros { get; set; } = [];
    }
}
