using System;
using System.Collections.Generic;
using System.Text;

namespace EdiStudio.Models.Parsing
{
    /// <summary>
    /// Representa em memória um arquivo EDI processado conforme um layout,
    /// contendo os registros reconhecidos em ordem e em estrutura hierárquica.
    /// </summary>
    /// <remarks>
    /// A implementação atual não preserva linhas vazias, curtas ou com
    /// identificadores desconhecidos.
    /// </remarks>
    public class ArquivoEdiParseado
    {
        public string CaminhoArquivo { get; set; } = string.Empty;
        public List<RegistroEdiNode> RegistrosEmOrdem { get; set; } = [];
        public List<RegistroEdiNode> RegistrosRaiz { get; set; } = [];

    }
}
