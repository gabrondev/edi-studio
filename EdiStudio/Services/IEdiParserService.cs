using EdiStudio.Models;
using EdiStudio.Models.Parsing;


namespace EdiStudio.Services
{
    public interface IEdiParserService
    {
        ArquivoEdiParseado Parse(string caminhoArquivo, Layout layout);
    }
}
