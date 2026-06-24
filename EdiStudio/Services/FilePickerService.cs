using Microsoft.Win32;

namespace EdiStudio.Services
{
    internal class FilePickerService : IFilePickerService
    {
        public string? SelecionarArquivo(string filtro, string tituloJanela)
        {
            OpenFileDialog openFileDialog = new() 
            { 
                Title = tituloJanela,
                Filter  = filtro,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            bool? resultado = openFileDialog.ShowDialog();

            return resultado == true
                ? openFileDialog.FileName
                : null;
        }

        public string? SelecionarLocalParaSalvarArquivo(
            string filtro,
            string titulo,
            string nomeArquivoPadrao)
        {
            SaveFileDialog saveFileDialog = new()
            {
                Filter = filtro,
                Title = titulo,
                FileName = nomeArquivoPadrao,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            bool? resultado = saveFileDialog.ShowDialog();

            return resultado == true
                ? saveFileDialog.FileName
                : null;
        }
    }
}
