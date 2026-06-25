using EdiStudio.Models;
using EdiStudio.Commands;
using System.Windows.Input;
using System.Collections.ObjectModel;
using EdiStudio.Repositories;
using EdiStudio.Services;
using System.IO;
using EdiStudio.Models.Parsing;
using EdiStudio.Models.Rendering;

namespace EdiStudio.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel
    {
        private readonly ILayoutRepository _layoutRepository;
        private readonly IFilePickerService _filePickerService;
        private readonly ILayoutWizardService _layoutWizardService;
        private readonly IEdiParserService _ediParserService;
        private readonly IEdiDocumentRendererService _ediDocumentRendererService;

        private string _titulo = "EDI Studio";
        public string Titulo
        {
            get => _titulo;
            set
            {
                _titulo = value;
                OnPropertyChanged();
            }
        }

        private string _statusLayout = "Layout carregado: Nenhum";
        public string StatusLayout
        {
            get => _statusLayout;
            set
            {
                _statusLayout = value;
                OnPropertyChanged();
            }
        }

        private Layout _layoutAtual = new();
        public Layout LayoutAtual
        {
            get => _layoutAtual;
            set
            {
                _layoutAtual = value;
                OnPropertyChanged();
            }
        }

        private ArquivoEdiParseado? _arquivoEdiAtual;

        public ArquivoEdiParseado? ArquivoEdiAtual
        {
            get => _arquivoEdiAtual;
            set
            {
                _arquivoEdiAtual = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<LinhaDocumentoEdi> LinhasDocumento { get; } = [];

        private LinhaDocumentoEdi? _linhaDocumentoSelecionada;

        public LinhaDocumentoEdi? LinhaDocumentoSelecionada
        {
            get => _linhaDocumentoSelecionada;
            set
            {
                _linhaDocumentoSelecionada = value;
                OnPropertyChanged();

                if (value?.RegistroRelacionado is not null)
                {
                    RegistroSelecionado = value.RegistroRelacionado;
                }
            }
        }

        private RegistroEdiNode? _registroSelecionado;

        public RegistroEdiNode? RegistroSelecionado
        {
            get => _registroSelecionado;
            set
            {
                _registroSelecionado = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TipoRegistro> TiposRegistro { get; private set; } = [];

        public ICommand AbrirLayoutCommand { get; }
        public ICommand NovoLayoutCommand { get; }
        public ICommand AbrirArquivoEdiCommand { get; }

        public MainWindowViewModel()
        {
            _layoutRepository = new LayoutJsonRepository();
            _filePickerService = new FilePickerService();
            _layoutWizardService = new LayoutWizardService();
            _ediDocumentRendererService = new EdiDocumentRendererService();
            _ediParserService = new EdiParserService();
            
            Titulo = "EDI Studio";
            LayoutAtual.Nome = "Nenhum layout carregado";

            AbrirLayoutCommand = new RelayCommand(AbrirLayout);
            NovoLayoutCommand = new RelayCommand(NovoLayout);
            AbrirArquivoEdiCommand = new RelayCommand(AbrirArquivoEdi);
        }

        private void ExibirLayout(Layout layout)
        {
            LayoutAtual = layout;

            TiposRegistro.Clear();

            foreach(var tipoRegistro in LayoutAtual.TiposRegistro)
            {
                TiposRegistro.Add(tipoRegistro);
            }
        }

        private void AbrirLayout()
        {
            string? caminhoArquivo = 
                _filePickerService.SelecionarArquivo("Arquivos JSON (*.json)|*.json", "Selecionar JSON do Layout");

            if (caminhoArquivo is null)
                return;

            Layout layout = _layoutRepository.Carregar(caminhoArquivo);

            StatusLayout = $"Layout carregado: {layout.Nome}";
            ExibirLayout(layout);
        }

        private void NovoLayout()
        {
            Layout? layoutCriado = _layoutWizardService.CriarNovoLayout();

            if (layoutCriado is null) return;

            string? caminhoArquivo =
                _filePickerService.SelecionarLocalParaSalvarArquivo(
                    "Arquivos JSON (*.json)|*.json*",
                    "Salvar layout EDI",
                    GerarNomeArquivoPadrao(layoutCriado)
                );

            if (caminhoArquivo is null)
                return;

            _layoutRepository.Salvar(layoutCriado, caminhoArquivo);

            ExibirLayout(layoutCriado);

            StatusLayout = $"Layout carregado: {layoutCriado.Nome}";
        }

        private string GerarNomeArquivoPadrao(Layout layout)
        {
            string nomeArquivo = string.IsNullOrWhiteSpace(layout.Nome)
                ? "layout"
                : layout.Nome.Trim();

            foreach(char caractereInvalido in Path.GetInvalidFileNameChars())
            {
                nomeArquivo = nomeArquivo.Replace(caractereInvalido, '_');
            }

            return $"{nomeArquivo}.json";
        }

        private void AbrirArquivoEdi()
        {
            if (LayoutAtual is null || string.IsNullOrWhiteSpace(LayoutAtual.Nome))
            {
                StatusLayout = "Carregue um layout antes de abrir o arquivo EDI.";
                return;
            }

            string? caminhoArquivo =
                _filePickerService.SelecionarArquivo(
                        "Arquivos TXT (*.txt)|*.txt|Todos os arquivos (*.*)|*.*",
                        "Selecionar arquivo EDI"
                    );

            if (caminhoArquivo is null)
                return;

            ArquivoEdiAtual = _ediParserService.Parse(caminhoArquivo, LayoutAtual);

            DocumentoEdiRenderizado documento =
                _ediDocumentRendererService.Renderizar(ArquivoEdiAtual);

            LinhasDocumento.Clear();

            foreach(LinhaDocumentoEdi linha in documento.Linhas)
            {
                LinhasDocumento.Add(linha);
            }

            StatusLayout = $"Layout carregado: {LayoutAtual.Nome} | Registros: {ArquivoEdiAtual.RegistrosEmOrdem.Count}";
        }
    }
}
