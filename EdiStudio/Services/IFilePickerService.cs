namespace EdiStudio.Services
{
    internal interface IFilePickerService
    {
        string? SelecionarArquivo(string filtro, string tituloJanela);

        string? SelecionarLocalParaSalvarArquivo(
            string filtro,
            string titulo,
            string nomeArquivoPadrao);
    }
}
