using EdiStudio.Models;
using System.IO;
using System.Text.Json;

namespace EdiStudio.Services
{
    public class ArquivosRecentesJsonService : IArquivosRecentesService
    {
        private readonly string _caminhoArquivo;

        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            WriteIndented = true
        };

        public ArquivosRecentesJsonService()
        {
            string pastaAppData = Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData
            );

            string pastaEdiStudio = Path.Combine(pastaAppData, "EdiStudio");

            Directory.CreateDirectory(pastaEdiStudio);

            _caminhoArquivo = Path.Combine(
                pastaEdiStudio,
                "recent-files.json"
            );
        }

        public ArquivosRecentesData Carregar()
        {
            if (!File.Exists(_caminhoArquivo))
                return new ArquivosRecentesData();

            string json = File.ReadAllText(_caminhoArquivo);

            ArquivosRecentesData? state =
                JsonSerializer.Deserialize<ArquivosRecentesData>(
                    json,
                    _jsonSerializerOptions
                );

            return state ?? new ArquivosRecentesData();
        }

        public void Salvar(ArquivosRecentesData state)
        {
            string json = JsonSerializer.Serialize(
                state,
                _jsonSerializerOptions
            );

            File.WriteAllText(_caminhoArquivo, json);
        }
    }
}