using EdiStudio.Enums;

namespace EdiStudio.Models
{
    internal class Campo
    {
        public string Nome { get; set; } = string.Empty;
        public FormatoCampo Formato {  get; set; }
        public int Tamanho { get; set; }
        public int Posicao { get; set; }
        public StatusCampo Status { get; set; }
    }
}
