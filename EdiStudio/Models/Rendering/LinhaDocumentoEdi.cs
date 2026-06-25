using EdiStudio.Enums;
using EdiStudio.Models.Parsing;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdiStudio.Models.Rendering
{
    public class LinhaDocumentoEdi
    {
        public string Texto {  get; set; } = string.Empty;
        public TipoLinhaDocumento TipoLinha { get; set; }
        public RegistroEdiNode? RegistroRelacionado { get; set; }
        public bool EhSelecionavel => RegistroRelacionado is not null;
    }
}
