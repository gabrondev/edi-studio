using System;
using System.Collections.Generic;
using System.Text;

namespace EdiStudio.Models.Parsing
{
    public class CampoEdiValor
    {
        public Campo CampoLayout { get; set; } = new();
        public string ValorOriginal { get; set; } = string.Empty;
        public string ValorAtual {  get; set; } = string.Empty;
        public bool FoiAlterado => ValorAtual != ValorOriginal;
    }
}
