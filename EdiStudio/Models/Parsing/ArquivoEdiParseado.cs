using System;
using System.Collections.Generic;
using System.Text;

namespace EdiStudio.Models.Parsing
{
    public class ArquivoEdiParseado
    {
        public string CaminhoArquivo { get; set; } = string.Empty;
        public List<RegistroEdiNode> RegistrosEmOrdem { get; set; } = [];
        public List<RegistroEdiNode> RegistrosRaiz { get; set; } = [];

    }
}
