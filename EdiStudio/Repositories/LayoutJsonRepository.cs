using EdiStudio.Models;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EdiStudio.Repositories
{
    internal class LayoutJsonRepository : ILayoutRepository
    {
        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public void Salvar(Layout layout, string caminhoArquivo)
        {
            string json = JsonSerializer.Serialize(layout, _jsonSerializerOptions);

            File.WriteAllText(caminhoArquivo, json);
        }

        public Layout Carregar(string caminhoArquivo)
        {
            string json = File.ReadAllText(caminhoArquivo);

            Layout? layout = JsonSerializer.Deserialize<Layout>(
                json,
                _jsonSerializerOptions);

            return layout ?? throw new InvalidOperationException("Não foi possível carregar o layout.");
        }
    }
}