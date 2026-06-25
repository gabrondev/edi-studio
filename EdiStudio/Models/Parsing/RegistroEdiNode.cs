using System;
using System.Collections.Generic;
using System.Text;

namespace EdiStudio.Models.Parsing
{
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
