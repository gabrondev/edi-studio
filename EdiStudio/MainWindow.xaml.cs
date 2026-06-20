using EdiStudio.Enums;
using EdiStudio.Models;
using EdiStudio.Repositories;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EdiStudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Layout layout = new() { Nome = "OCOREN Wella" };

            // TIPOS DE REGISTRO DO LAYOUT
            TipoRegistro registro540 = new()
            {
                Identificador = 540,
                IdentificadorRegistroPai = 000,
                Nome = "Cabeçalho do Documento",
                QtdeMaxOcorrenciasPorRegistroPai = 200,
                TamanhoTipoRegistro = 250
            };

            TipoRegistro registro541 = new()
            {
                Identificador = 541,
                IdentificadorRegistroPai = 540,
                Nome = "Dados da Transportadora",
                QtdeMaxOcorrenciasPorRegistroPai = 1,
                TamanhoTipoRegistro = 250
            };

            TipoRegistro registro542 = new()
            {
                Identificador = 542,
                IdentificadorRegistroPai = 541,
                Nome = "Ocorrência na Entrega",
                QtdeMaxOcorrenciasPorRegistroPai = 9999,
                TamanhoTipoRegistro = 250
            };

            // CAMPOS DO REGISTRO 542
            registro542.Campos.Add(new Campo
            {
                Nome = "CNPJ Emissor NF",
                Formato = FormatoCampo.Numerico,
                Tamanho = 14,
                Posicao = 4,
                Status = StatusCampo.Obrigatorio
            });

            registro542.Campos.Add(new Campo
            {
                Nome = "Número da Nota Fiscal",
                Formato = FormatoCampo.Numerico,
                Tamanho = 9,
                Posicao = 21,
                Status = StatusCampo.Obrigatorio
            });

            registro542.Campos.Add(new Campo
            {
                Nome = "Código de Ocorrência",
                Formato = FormatoCampo.Numerico,
                Tamanho = 3,
                Posicao = 30,
                Status = StatusCampo.Obrigatorio
            });

            // ADICIONANDO OS TIPOS DE REGISTRO AO LAYOUT
            layout.TiposRegistro.Add(registro540);
            layout.TiposRegistro.Add(registro541);
            layout.TiposRegistro.Add(registro542);

            // REPOSITORY
            ILayoutRepository repositorio = new LayoutJsonRepository();
            
            repositorio.Salvar(layout, @"C:\Temp\Wella.json");

            Layout layoutCarregado = repositorio.Carregar(@"C:\Temp\Wella.json");

            // Breakpoint


        }
    }
}