using EdiStudio.Models;
using EdiStudio.Models.Parsing;
using EdiStudio.Services;
using System.IO;

namespace EdiStudio.Tests.Services
{
    public class EdiParserServiceTests
    {
        private string? _caminhoArquivoTemporario;

        private string CriarArquivoTemporario(params string[] linhas)
        {
            // Cria um arquivo temporário com nome único para não haver conflitos entre diferentes testes.
            _caminhoArquivoTemporario = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.txt");

            File.WriteAllLines(_caminhoArquivoTemporario, linhas);
            
            return _caminhoArquivoTemporario;
        }

        private static Layout CriarLayoutSimples()
        {
            Campo campoIdentificador = new()
            {
                Nome = "Identificador",
                Posicao = 1,
                Tamanho = 3
            };

            Campo campoConteudo = new()
            {
                Nome = "Conteudo",
                Posicao = 4,
                Tamanho = 5
            };

            TipoRegistro tipoRegistro = new()
            {
                Identificador = "001",
                Nome = "Cabeçalho",
                TamanhoTipoRegistro = 8,
                Campos = [campoIdentificador, campoConteudo]
            };

            return new Layout
            {
                Nome = "Layout de teste",
                TiposRegistro = [tipoRegistro]
            };
        }

        [TearDown]
        public void ExcluirArquivoTemporario()
        {
            if(_caminhoArquivoTemporario is not null &&
                File.Exists(_caminhoArquivoTemporario))
            {
                File.Delete(_caminhoArquivoTemporario);
            }
        }

        [Test]
        public void Parse_QuandoLinhaCorrespondeAoLayout_ExtraiRegistroECampos()
        {
            // Arrange
            Layout layout = CriarLayoutSimples();
            string caminhoArquivo = CriarArquivoTemporario("001ABCDE");
            EdiParserService parser = new();

            // Act
            ArquivoEdiParseado resultado = parser.Parse(_caminhoArquivoTemporario, layout);

            // Assert
            Assert.That(resultado.RegistrosEmOrdem, Has.Count.EqualTo(1));

            RegistroEdiNode registro = resultado.RegistrosEmOrdem[0];

            Assert.That(registro.NumeroLinha, Is.EqualTo(1));
            Assert.That(registro.LinhaOriginal, Is.EqualTo("001ABCDE"));
            Assert.That(registro.TipoRegistro, Is.SameAs(layout.TiposRegistro[0]));
            Assert.That(registro.Campos, Has.Count.EqualTo(2));

            Assert.That(registro.Campos[1].ValorAtual, Is.EqualTo("ABCDE"));

            Assert.That(resultado.RegistrosRaiz, Does.Contain(registro));
        }

        [Test]
        public void Parse_QuandoCampoEstaIncompleto_PreencheComEspacos()
        {
            Layout layout = CriarLayoutSimples();
            string caminhoArquivo = CriarArquivoTemporario("001AB");

            EdiParserService parser = new();

            ArquivoEdiParseado resultado = parser.Parse(caminhoArquivo, layout);

            CampoEdiValor campoConteudo = resultado.RegistrosEmOrdem[0].Campos[1];

            Assert.That(campoConteudo.ValorOriginal, Is.EqualTo("AB   "));
            Assert.That(campoConteudo.ValorAtual, Is.EqualTo("AB   "));
        }

        [Test]
        public void Parse_QuandoLinhaNaoPodeSerInterpretada_IgnoraLinha()
        {
            Layout layout = CriarLayoutSimples();
            string caminhoArquivo = CriarArquivoTemporario(
                "",
                "AB",
                "999ABCDE",
                "001ABCDE"
                );

            EdiParserService parser = new();

            ArquivoEdiParseado resultado = parser.Parse(caminhoArquivo, layout);

            Assert.That(resultado.RegistrosEmOrdem[0].NumeroLinha, Is.EqualTo(4));
        }

        [Test]
        public void Parse_QuandoTipoPossuiPai_AssociaRegistroAoPai()
        {
            TipoRegistro definicaoPai = new()
            {
                Identificador = "001",
                Nome = "Pai"
            };

            TipoRegistro definicaoFilho = new()
            {
                Identificador = "002",
                IdentificadorRegistroPai = "001",
                Nome = "Filho"
            };

            Layout layout = new()
            {
                TiposRegistro = [definicaoPai, definicaoFilho]
            };

            string caminhoArquivo = CriarArquivoTemporario("001", "002");

            EdiParserService parser = new();

            ArquivoEdiParseado resultado = parser.Parse(caminhoArquivo, layout);

            RegistroEdiNode registroPai = resultado.RegistrosEmOrdem[0];
            RegistroEdiNode registroFilho = resultado.RegistrosEmOrdem[1];

            Assert.That(registroFilho.Pai, Is.SameAs(registroPai));
            Assert.That(registroPai.Filhos, Does.Contain(registroFilho));
            Assert.That(resultado.RegistrosRaiz, Does.Contain(registroPai));
            Assert.That(resultado.RegistrosRaiz, Does.Not.Contain(registroFilho));
        }
    }
}
