using EdiStudio.Models;
using System.IO;
using System.Text.Json;

namespace EdiStudio.Repositories
{
    internal class LayoutJsonRepository : ILayoutRepository
    {
        public void Salvar(Layout layout, string caminhoArquivo)
        {
            string json = JsonSerializer.Serialize(
                layout,
                new JsonSerializerOptions { WriteIndented = true }
            );

            File.WriteAllText(caminhoArquivo, json);
        }

        public Layout Carregar(string caminhoArquivo)
        {
            string json = File.ReadAllText(caminhoArquivo);

            return JsonSerializer.Deserialize<Layout>(json)!;
        }
    }
}