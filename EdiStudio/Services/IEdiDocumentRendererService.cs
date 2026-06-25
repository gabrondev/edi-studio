using EdiStudio.Models.Parsing;
using EdiStudio.Models.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdiStudio.Services
{
    public interface IEdiDocumentRendererService
    {
        DocumentoEdiRenderizado Renderizar(ArquivoEdiParseado arquivo);
    }
}
