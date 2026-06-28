using EdiStudio.Commands;
using EdiStudio.Models;
using EdiStudio.Models.Parsing;
using EdiStudio.Models.Rendering;
using EdiStudio.Repositories;
using EdiStudio.Services;
using EdiStudio.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace EdiStudio.ViewModels
{
    internal class MainWindowViewModel : BaseViewModel
    {
        private readonly ILayoutRepository _layoutRepository;
        private readonly IFilePickerService _filePickerService;
        private readonly ILayoutWizardService _layoutWizardService;
        private readonly IEdiParserService _ediParserService;
        private readonly IEdiDocumentRendererService _ediDocumentRendererService;
        private readonly IArquivosRecentesService _arquivosRecentesService;
        private readonly IEdiWriterService _ediWriterService;

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

        private string? _caminhoLayoutAtual;

        public ObservableCollection<LinhaDocumentoEdi> LinhasDocumento { get; } = [];

        private LinhaDocumentoEdi? _linhaDocumentoSelecionada;

        public LinhaDocumentoEdi? LinhaDocumentoSelecionada
        {
            get => _linhaDocumentoSelecionada;
            set
            {
                _linhaDocumentoSelecionada = value;
                OnPropertyChanged();

                RegistroSelecionado = value?.RegistroRelacionado;
                _excluirRegistroSelecionadoCommand.RaiseCanExecuteChanged();
            }
        }

        private void ExcluirRegistroSelecionado()
        {
            if (ArquivoEdiAtual is null || RegistroSelecionado is null)
                return;

            RegistroEdiNode selecionado = RegistroSelecionado;
            List<RegistroEdiNode> cascata = ObterRegistroEDescendentes(selecionado);

            ConfirmarExclusaoWindow dialogo = new(
                selecionado.NumeroLinha,
                cascata.Count - 1
            );

            if (Application.Current?.MainWindow is Window owner)
                dialogo.Owner = owner;

            if (dialogo.ShowDialog() != true)
                return;

            int indiceAnterior =
                ArquivoEdiAtual.RegistrosEmOrdem.IndexOf(selecionado);

            HashSet<RegistroEdiNode> registrosParaExcluir =
                dialogo.ExcluirSomenteSelecionado
                    ? new HashSet<RegistroEdiNode> { selecionado }
                    : cascata.ToHashSet();

            if (dialogo.ExcluirSomenteSelecionado)
            {
                foreach (RegistroEdiNode filho in selecionado.Filhos)
                    filho.Pai = null;

                selecionado.Filhos.Clear();
            }

            foreach (RegistroEdiNode registro in registrosParaExcluir)
                registro.Pai?.Filhos.Remove(registro);

            ArquivoEdiAtual.RegistrosEmOrdem.RemoveAll(
                registrosParaExcluir.Contains
            );

            for (int i = 0; i < ArquivoEdiAtual.RegistrosEmOrdem.Count; i++)
                ArquivoEdiAtual.RegistrosEmOrdem[i].NumeroLinha = i + 1;

            ArquivoEdiAtual.RegistrosRaiz = ArquivoEdiAtual.RegistrosEmOrdem
                .Where(registro => registro.Pai is null)
                .ToList();

            LinhaDocumentoSelecionada = null;

            DocumentoEdiRenderizado documento =
                _ediDocumentRendererService.Renderizar(ArquivoEdiAtual);

            LinhasDocumento.Clear();

            foreach (LinhaDocumentoEdi linha in documento.Linhas)
                LinhasDocumento.Add(linha);

            SelecionarRegistroProximo(indiceAnterior);

            StatusLayout =
                $"{registrosParaExcluir.Count} linha(s) excluída(s) | " +
                $"Registros: {ArquivoEdiAtual.RegistrosEmOrdem.Count}";
        }

        private List<RegistroEdiNode> ObterRegistroEDescendentes(
            RegistroEdiNode registro)
        {
            HashSet<RegistroEdiNode> encontrados = [];

            void Percorrer(RegistroEdiNode atual)
            {
                if (!encontrados.Add(atual))
                    return;

                foreach (RegistroEdiNode filho in atual.Filhos)
                    Percorrer(filho);
            }

            Percorrer(registro);

            return ArquivoEdiAtual!.RegistrosEmOrdem
                .Where(encontrados.Contains)
                .ToList();
        }

        private void SelecionarRegistroProximo(int indiceAnterior)
        {
            if (ArquivoEdiAtual is null ||
                ArquivoEdiAtual.RegistrosEmOrdem.Count == 0)
                return;

            int indice = Math.Min(
                indiceAnterior,
                ArquivoEdiAtual.RegistrosEmOrdem.Count - 1
            );

            RegistroEdiNode proximo =
                ArquivoEdiAtual.RegistrosEmOrdem[indice];

            LinhaDocumentoSelecionada = LinhasDocumento.FirstOrDefault(
                linha => ReferenceEquals(linha.RegistroRelacionado, proximo)
            );
        }

        private RegistroEdiNode? _registroSelecionado;

        public RegistroEdiNode? RegistroSelecionado
        {
            get => _registroSelecionado;
            set
            {
                if (_registroSelecionado == value)
                    return;

                DesassinarEventosDoRegistro(_registroSelecionado);
                _registroSelecionado = value;
                AssinarEventosDoRegistro(_registroSelecionado);
                OnPropertyChanged();
            }
        }

        private GridLength _larguraPainelEditor = new(360);
        private GridLength _larguraSplitterEditor = new(5);
        private GridLength _larguraAbaEditor = new(0);
        private GridLength _larguraAnteriorPainelEditor = new(360);

        private bool _editorEstaVisivel = true;

        public GridLength LarguraPainelEditor
        {
            get => _larguraPainelEditor;
            set
            {
                _larguraPainelEditor = value;
                OnPropertyChanged();

                if (_editorEstaVisivel && value.Value > 0)
                    _larguraAnteriorPainelEditor = value;
            }
        }

        public GridLength LarguraSplitterEditor
        {
            get => _larguraSplitterEditor;
            set
            {
                _larguraSplitterEditor = value;
                OnPropertyChanged();
            }
        }

        public GridLength LarguraAbaEditor
        {
            get => _larguraAbaEditor;
            set
            {
                _larguraAbaEditor = value;
                OnPropertyChanged();
            }
        }

        public bool EditorEstaVisivel
        {
            get => _editorEstaVisivel;
            set
            {
                _editorEstaVisivel = value;
                OnPropertyChanged();
            }
        }
        
        public ObservableCollection<TipoRegistro> TiposRegistro { get; private set; } = [];
        public ObservableCollection<ArquivoEdiRecente> ArquivosEdiRecentes { get; } = [];
        public ObservableCollection<LayoutRecente> LayoutsRecentes { get; } = [];

        public ICommand AlternarEditorCommand { get; }
        public ICommand AbrirLayoutCommand { get; }
        public ICommand NovoLayoutCommand { get; }
        public ICommand AbrirArquivoEdiCommand { get; }
        public ICommand AbrirArquivoEdiRecenteCommand { get; }
        public ICommand AbrirLayoutRecenteCommand { get; }
        public ICommand SalvarArquivoEdiCommand { get; }

        public ICommand SalvarArquivoEdiComoCommand { get; }
        private readonly RelayCommand _excluirRegistroSelecionadoCommand;

        public ICommand ExcluirRegistroSelecionadoCommand =>
            _excluirRegistroSelecionadoCommand;

        public MainWindowViewModel()
        {
            _layoutRepository = new LayoutJsonRepository();
            _filePickerService = new FilePickerService();
            _layoutWizardService = new LayoutWizardService();
            _ediDocumentRendererService = new EdiDocumentRendererService();
            _ediParserService = new EdiParserService();
            _arquivosRecentesService = new ArquivosRecentesJsonService();
            _ediWriterService = new EdiWriterService();


            LayoutAtual.Nome = "Nenhum layout carregado";

            AbrirLayoutCommand = new RelayCommand(AbrirLayout);
            NovoLayoutCommand = new RelayCommand(NovoLayout);
            AbrirArquivoEdiCommand = new RelayCommand(AbrirArquivoEdi);
            AbrirArquivoEdiRecenteCommand =
                new RelayCommandComParametro<ArquivoEdiRecente>(AbrirArquivoEdiRecente);

            AbrirLayoutRecenteCommand =
                new RelayCommandComParametro<LayoutRecente>(AbrirLayoutRecente);

            AlternarEditorCommand = new RelayCommand(AlternarEditor);

            SalvarArquivoEdiCommand = new RelayCommand(SalvarArquivoEdi);

            SalvarArquivoEdiComoCommand = new RelayCommand(SalvarArquivoEdiComo);

            _excluirRegistroSelecionadoCommand = new RelayCommand(
                ExcluirRegistroSelecionado,
                () => ArquivoEdiAtual is not null && RegistroSelecionado is not null
            );

            CarregarArquivosRecentes();
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
                _filePickerService.SelecionarArquivo(
                    "Arquivos JSON (*.json)|*.json",
                    "Selecionar layout EDI"
                );

            if (caminhoArquivo is null)
                return;

            CarregarLayout(caminhoArquivo);

            AdicionarLayoutRecente(caminhoArquivo);
        }

        private void CarregarLayout(string caminhoLayout)
        {
            Layout layout = _layoutRepository.Carregar(caminhoLayout);

            _caminhoLayoutAtual = caminhoLayout;

            ExibirLayout(layout);

            StatusLayout = $"Layout carregado: {layout.Nome}";
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

            _caminhoLayoutAtual = caminhoArquivo;

            AdicionarLayoutRecente(caminhoArquivo);

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
            if (LayoutAtual is null || string.IsNullOrWhiteSpace(_caminhoLayoutAtual))
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

            CarregarArquivoEdi(caminhoArquivo);

            AdicionarArquivoEdiRecente(caminhoArquivo, _caminhoLayoutAtual);
        }

        private void CarregarArquivoEdi(string caminhoArquivo)
        {
            if (LayoutAtual is null)
            {
                StatusLayout = "Carregue um layout antes de abrir o arquivo EDI.";
                return;
            }

            ArquivoEdiAtual = _ediParserService.Parse(caminhoArquivo, LayoutAtual);

            DocumentoEdiRenderizado documento =
                _ediDocumentRendererService.Renderizar(ArquivoEdiAtual);

            LinhasDocumento.Clear();

            foreach (LinhaDocumentoEdi linha in documento.Linhas)
            {
                LinhasDocumento.Add(linha);
            }

            StatusLayout =
                $"Layout carregado: {LayoutAtual.Nome} | Registros: {ArquivoEdiAtual.RegistrosEmOrdem.Count}";
        }

        private void AbrirArquivoEdiRecente(ArquivoEdiRecente? arquivoRecente)
        {
            if (arquivoRecente is null)
                return;

            if (!File.Exists(arquivoRecente.CaminhoArquivoEdi))
            {
                ArquivosEdiRecentes.Remove(arquivoRecente);
                SalvarArquivosRecentes();

                StatusLayout = "Arquivo EDI recente não encontrado.";
                return;
            }

            if (!File.Exists(arquivoRecente.CaminhoLayout))
            {
                ArquivosEdiRecentes.Remove(arquivoRecente);
                SalvarArquivosRecentes();

                StatusLayout = "Layout associado ao arquivo EDI recente não encontrado.";
                return;
            }

            CarregarLayout(arquivoRecente.CaminhoLayout);

            CarregarArquivoEdi(arquivoRecente.CaminhoArquivoEdi);

            AdicionarArquivoEdiRecente(
                arquivoRecente.CaminhoArquivoEdi,
                arquivoRecente.CaminhoLayout
            );
        }

        private void AssinarEventosDoRegistro(RegistroEdiNode? registro)
        {
            if (registro is null)
                return;

            foreach(CampoEdiValor campo in registro.Campos)
            {
                campo.PropertyChanged += CampoSelecionado_PropertyChanged;
            }
        }

        private void DesassinarEventosDoRegistro(RegistroEdiNode? registro)
        {
            if (registro is null)
                return;

            foreach(CampoEdiValor campo in registro.Campos)
            {
                campo.PropertyChanged -= CampoSelecionado_PropertyChanged;
            }
        }

        private void CampoSelecionado_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(CampoEdiValor.ValorAtual))
                return;

            if (RegistroSelecionado is null)
                return;

            AtualizarLinhaDocumentoDoRegistro(RegistroSelecionado);
        }

        private void AtualizarLinhaDocumentoDoRegistro(RegistroEdiNode registro)
        {
            LinhaDocumentoEdi? linhaDocumento = LinhasDocumento
                .FirstOrDefault(linha =>
                    ReferenceEquals(linha.RegistroRelacionado, registro));

            if (linhaDocumento is null)
                return;

            linhaDocumento.Texto =
                _ediDocumentRendererService.RenderizarLinhaRegistro(registro);
        }

        private void AlternarEditor()
        {
            if (EditorEstaVisivel)
            {
                if (LarguraPainelEditor.Value > 0)
                    _larguraAnteriorPainelEditor = LarguraPainelEditor;

                LarguraPainelEditor = new GridLength(0);
                LarguraSplitterEditor = new GridLength(0);
                LarguraAbaEditor = new GridLength(32);
                EditorEstaVisivel = false;

                return;
            }

            LarguraPainelEditor = _larguraAnteriorPainelEditor.Value > 0
                ? _larguraAnteriorPainelEditor
                : new GridLength(360);

            LarguraSplitterEditor = new GridLength(5);
            LarguraAbaEditor = new GridLength(0);
            EditorEstaVisivel = true;
        }

        private void AdicionarArquivoEdiRecente(
            string caminhoArquivoEdi,
            string caminhoLayout)
        {
            ArquivoEdiRecente? existente = ArquivosEdiRecentes
                .FirstOrDefault(item => item.CaminhoArquivoEdi == caminhoArquivoEdi);

            if (existente is not null)
                ArquivosEdiRecentes.Remove(existente);

            ArquivosEdiRecentes.Insert(0, new ArquivoEdiRecente
            {
                NomeExibicao = Path.GetFileName(caminhoArquivoEdi),
                CaminhoArquivoEdi = caminhoArquivoEdi,
                CaminhoLayout = caminhoLayout
            });

            const int limite = 10;

            while (ArquivosEdiRecentes.Count > limite)
            {
                ArquivosEdiRecentes.RemoveAt(ArquivosEdiRecentes.Count - 1);
            }

            SalvarArquivosRecentes();
        }

        private void AdicionarLayoutRecente(string caminhoLayout)
        {
            LayoutRecente? existente = LayoutsRecentes
                .FirstOrDefault(item => item.CaminhoLayout == caminhoLayout);

            if (existente is not null)
                LayoutsRecentes.Remove(existente);

            LayoutsRecentes.Insert(0, new LayoutRecente
            {
                NomeExibicao = Path.GetFileName(caminhoLayout),
                CaminhoLayout = caminhoLayout
            });

            const int limite = 10;

            while (LayoutsRecentes.Count > limite)
            {
                LayoutsRecentes.RemoveAt(LayoutsRecentes.Count - 1);
            }

            SalvarArquivosRecentes();
        }

        private void AbrirLayoutRecente(LayoutRecente? layoutRecente)
        {
            if (layoutRecente is null)
                return;

            if (!File.Exists(layoutRecente.CaminhoLayout))
            {
                LayoutsRecentes.Remove(layoutRecente);
                SalvarArquivosRecentes();

                StatusLayout = "Layout recente não encontrado.";
                return;
            }

            Layout layout = _layoutRepository.Carregar(layoutRecente.CaminhoLayout);

            _caminhoLayoutAtual = layoutRecente.CaminhoLayout;

            ExibirLayout(layout);

            AdicionarLayoutRecente(layoutRecente.CaminhoLayout);

            StatusLayout = $"Layout carregado: {layout.Nome}";
        }

        private void CarregarArquivosRecentes()
        {
            ArquivosRecentesData data = _arquivosRecentesService.Carregar();

            ArquivosEdiRecentes.Clear();

            foreach (ArquivoEdiRecente arquivo in data.ArquivosEdiRecentes)
            {
                ArquivosEdiRecentes.Add(arquivo);
            }

            LayoutsRecentes.Clear();

            foreach (LayoutRecente layout in data.LayoutsRecentes)
            {
                LayoutsRecentes.Add(layout);
            }
        }

        private void SalvarArquivosRecentes()
        {
            ArquivosRecentesData data = new()
            {
                ArquivosEdiRecentes = ArquivosEdiRecentes.ToList(),
                LayoutsRecentes = LayoutsRecentes.ToList()
            };

            _arquivosRecentesService.Salvar(data);
        }

        private void SalvarArquivoEdi()
        {
            if (ArquivoEdiAtual is null)
            {
                StatusLayout = "Nenhum arquivo EDI aberto para salvar.";
                return;
            }

            if (string.IsNullOrWhiteSpace(ArquivoEdiAtual.CaminhoArquivo))
            {
                SalvarArquivoEdiComo();
                return;
            }

            SalvarArquivoEdiEm(ArquivoEdiAtual.CaminhoArquivo);
        }

        private void SalvarArquivoEdiComo()
        {
            if (ArquivoEdiAtual is null)
            {
                StatusLayout = "Nenhum arquivo EDI aberto para salvar.";
                return;
            }

            string nomeArquivoPadrao = string.IsNullOrWhiteSpace(ArquivoEdiAtual.CaminhoArquivo)
                ? "arquivo_edi.txt"
                : Path.GetFileName(ArquivoEdiAtual.CaminhoArquivo);

            string? caminhoArquivo =
                _filePickerService.SelecionarLocalParaSalvarArquivo(
                    "Arquivos TXT (*.txt)|*.txt|Todos os arquivos (*.*)|*.*",
                    "Salvar arquivo EDI",
                    nomeArquivoPadrao
                );

            if (caminhoArquivo is null)
                return;

            SalvarArquivoEdiEm(caminhoArquivo);
        }

        private void SalvarArquivoEdiEm(string caminhoArquivo)
        {
            if (ArquivoEdiAtual is null)
                return;

            _ediWriterService.Salvar(ArquivoEdiAtual, caminhoArquivo);

            ArquivoEdiAtual.CaminhoArquivo = caminhoArquivo;

            ConfirmarAlteracoesComoSalvas(ArquivoEdiAtual);

            if (!string.IsNullOrWhiteSpace(_caminhoLayoutAtual))
            {
                AdicionarArquivoEdiRecente(
                    caminhoArquivo,
                    _caminhoLayoutAtual
                );
            }

            StatusLayout = $"Arquivo EDI salvo: {Path.GetFileName(caminhoArquivo)}";
        }

        private void ConfirmarAlteracoesComoSalvas(ArquivoEdiParseado arquivo)
        {
            foreach (RegistroEdiNode registro in arquivo.RegistrosEmOrdem)
            {
                foreach (CampoEdiValor campo in registro.Campos)
                {
                    campo.ConfirmarValorAtualComoOriginal();
                }
            }
        }
    }
}
