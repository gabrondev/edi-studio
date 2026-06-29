using EdiStudio.Enums;
using EdiStudio.Models;
using EdiStudio.Models.Parsing;
using EdiStudio.Services;
using System.IO;

namespace EdiStudio.Tests.Services
{
    internal class EdiWriterServiceTests
    {
        private string? _caminhoArquivoTemporario;

        [TearDown]
        public void ExcluirArquivoTemporario()
        {
            if (_caminhoArquivoTemporario is not null &&
                File.Exists(_caminhoArquivoTemporario))
            {
                File.Delete(_caminhoArquivoTemporario);
            }
        }

        [Test]
        public void Salvar_QuandoCampoEhAlfanumerico_PreencheADireita()
        {
            Campo campo = new()
            {
                Posicao = 4,
                Tamanho = 5,
                Formato = FormatoCampo.Alfanumerico
            };

            ArquivoEdiParseado documento = CriarDocumento(
                campo,
                valorAtual: "AB",
                linhaOriginal: "001XXXXXFINAL"
                );

            _caminhoArquivoTemporario = CriarCaminhoTemporario();

            EdiWriterService writer = new();

            writer.Salvar(documento, _caminhoArquivoTemporario);

            string linhaSalva = File.ReadAllLines(_caminhoArquivoTemporario).Single();

            Assert.That(linhaSalva, Is.EqualTo("001AB   FINAL"));
        }

        [Test]
        public void Salvar_QuandoCampoEhNumerico_PreencheComZerosAEsquerda()
        {
            Campo campo = new()
            {
                Posicao = 4,
                Tamanho = 5,
                Formato = FormatoCampo.Numerico
            };

            ArquivoEdiParseado documento = CriarDocumento(
                campo,
                valorAtual: "123",
                linhaOriginal: "001XXXXX"
                );

            _caminhoArquivoTemporario = CriarCaminhoTemporario();

            EdiWriterService writer = new();

            writer.Salvar(documento, _caminhoArquivoTemporario);

            string linhaSalva = 
                File.ReadAllLines(_caminhoArquivoTemporario).Single();

            Assert.That(linhaSalva, Is.EqualTo("00100123"));
        }

        [Test]
        public void Salvar_QuandoCampoNumericoEstaVazio_PreencheComEspacos()
        {
            Campo campo = new()
            {
                Posicao = 4,
                Tamanho = 5,
                Formato = FormatoCampo.Numerico
            };

            ArquivoEdiParseado documento = CriarDocumento(
                campo,
                valorAtual: string.Empty,
                linhaOriginal: "001XXXXX"
                );

            _caminhoArquivoTemporario = CriarCaminhoTemporario();

            EdiWriterService writer= new();

            writer.Salvar(documento, _caminhoArquivoTemporario);

            string linhaSalva =
                File.ReadAllLines(_caminhoArquivoTemporario).Single();

            Assert.That(linhaSalva, Is.EqualTo("001     "));
        }

        private string? CriarCaminhoTemporario()
        {
            return Path.Combine(
                Path.GetTempPath(),
                $"{Guid.NewGuid():N}.txt"
                );
        }

        private static ArquivoEdiParseado CriarDocumento(Campo campo, string valorAtual, string linhaOriginal)
        {
            TipoRegistro tipoRegistro = new()
            {
                Identificador = "001",
                Nome = "Registro de teste",
                TamanhoTipoRegistro = linhaOriginal.Length,
                Campos = [campo]
            };

            CampoEdiValor valorCampo = new()
            {
                CampoLayout = campo,
                ValorOriginal = new string(' ', campo.Tamanho),
                ValorAtual = valorAtual
            };

            RegistroEdiNode registro = new()
            {
                NumeroLinha = 1,
                LinhaOriginal = linhaOriginal,
                TipoRegistro = tipoRegistro,
                Campos = [valorCampo]
            };

            return new ArquivoEdiParseado
            {
                RegistrosEmOrdem = [registro],
                RegistrosRaiz = [registro]
            };
        }
    }
}
