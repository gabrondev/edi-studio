using EdiStudio.Models;

namespace EdiStudio.Repositories
{
    internal interface ILayoutRepository
    {
        void Salvar(Layout layout, string caminhoArquivo);
        Layout Carregar(string caminhoArquivo);
    }
}
