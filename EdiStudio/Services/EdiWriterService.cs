using EdiStudio.Enums;
using EdiStudio.Models;
using EdiStudio.Models.Parsing;
using System.IO;

namespace EdiStudio.Services
{
    /// <summary>
    /// Serializa um documento EDI em linhas de largura fixa e grava o
    /// resultado em um arquivo TXT.
    /// </summary>
    /// <remarks>
    /// Os campos conhecidos são sobrescritos conforme suas posições no
    /// layout. Caracteres da linha original fora dessas posições são 
    /// preservados pela implementação atual.
    /// </remarks>
    public class EdiWriterService : IEdiWriterService
    {
        public void Salvar(ArquivoEdiParseado arquivo, string caminhoArquivo)
        {
            List<string> linhas = arquivo.RegistrosEmOrdem
                .Select(MontarLinhaRegistro)
                .ToList();

            File.WriteAllLines(caminhoArquivo, linhas);
        }

        private string MontarLinhaRegistro(RegistroEdiNode registro)
        {
            List<Campo> campos = registro.TipoRegistro.Campos
                .OrderBy(campo => campo.Posicao)
                .ToList();

            int tamanhoLinha = CalcularTamanhoLinha(registro, campos);

            char[] caracteres = CriarLinhaBase(registro.LinhaOriginal, tamanhoLinha);

            foreach (Campo campo in campos)
            {
                CampoEdiValor? campoValor = registro.Campos
                    .FirstOrDefault(valor => valor.CampoLayout == campo);

                string valor = campoValor?.ValorAtual ?? string.Empty;

                string valorAjustado = AjustarValorParaCampo(valor, campo);

                int indiceInicial = campo.Posicao - 1;

                for (int i = 0; i < campo.Tamanho; i++)
                {
                    caracteres[indiceInicial + i] = valorAjustado[i];
                }
            }

            return new string(caracteres);
        }

        private int CalcularTamanhoLinha(
            RegistroEdiNode registro,
            List<Campo> campos)
        {
            int tamanhoTipoRegistro = registro.TipoRegistro.TamanhoTipoRegistro;

            int tamanhoPelosCampos = campos.Count == 0
                ? 0
                : campos.Max(campo => campo.Posicao - 1 + campo.Tamanho);

            int tamanhoLinhaOriginal = registro.LinhaOriginal.Length;

            return Math.Max(
                Math.Max(tamanhoTipoRegistro, tamanhoPelosCampos),
                tamanhoLinhaOriginal
            );
        }

        private char[] CriarLinhaBase(string linhaOriginal, int tamanhoLinha)
        {
            char[] caracteres = new string(' ', tamanhoLinha).ToCharArray();

            int tamanhoParaCopiar = Math.Min(
                linhaOriginal.Length,
                tamanhoLinha
            );

            for (int i = 0; i < tamanhoParaCopiar; i++)
            {
                caracteres[i] = linhaOriginal[i];
            }

            return caracteres;
        }

        private string AjustarValorParaCampo(string valor, Campo campo)
        {
            valor ??= string.Empty;

            valor = valor.TrimEnd();

            if (valor.Length > campo.Tamanho)
            {
                return valor[..campo.Tamanho];
            }

            if (campo.Formato == FormatoCampo.Numerico)
            {
                string valorNumerico = valor.Trim();

                if (string.IsNullOrEmpty(valorNumerico))
                    return new string(' ', campo.Tamanho);

                return valorNumerico.PadLeft(campo.Tamanho, '0');
            }

            return valor.PadRight(campo.Tamanho);
        }
    }
}