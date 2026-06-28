using EdiStudio.Models.Parsing;

namespace EdiStudio.Services
{
    public interface IEdiWriterService
    {
        void Salvar(ArquivoEdiParseado arquivo, string caminhoArquivo);
    }
}