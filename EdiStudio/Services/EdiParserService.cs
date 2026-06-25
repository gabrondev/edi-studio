using EdiStudio.Models;
using EdiStudio.Models.Parsing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace EdiStudio.Services
{
    internal class EdiParserService : IEdiParserService
    {
        public ArquivoEdiParseado Parse(string caminhoArquivo, Layout layout)
        {
            string[] linhas = File.ReadAllLines(caminhoArquivo);

            ArquivoEdiParseado arquivo = new()
            {
                CaminhoArquivo = caminhoArquivo
            };

            Dictionary<string, RegistroEdiNode> ultimoRegistroPorTipo = [];

            for (int i = 0; i < linhas.Length; i++)
            {
                string linha = linhas[i];

                if (string.IsNullOrWhiteSpace(linha))
                    continue;

                RegistroEdiNode? registro = ParseLinha(
                    linha,
                    i + 1,
                    layout,
                    ultimoRegistroPorTipo
                );

                if (registro is null)
                    continue;

                arquivo.RegistrosEmOrdem.Add(registro);

                if (registro.Pai is null)
                    arquivo.RegistrosRaiz.Add(registro);
                else
                    registro.Pai.Filhos.Add(registro);

                ultimoRegistroPorTipo[registro.TipoRegistro.Identificador] = registro;
            }

            return arquivo;
        }

        private RegistroEdiNode? ParseLinha(
            string linha,
            int numeroLinha,
            Layout layout,
            Dictionary<string, RegistroEdiNode> ultimoRegistroPorTipo)
        {
            if (linha.Length < 3)
                return null;

            string identificador = linha[..3];

            TipoRegistro? tipoRegistro = layout.TiposRegistro
                .FirstOrDefault(tipo => tipo.Identificador == identificador);

            if (tipoRegistro is null)
                return null;

            RegistroEdiNode? pai = null;

            if (!string.IsNullOrWhiteSpace(tipoRegistro.IdentificadorRegistroPai))
            {
                ultimoRegistroPorTipo.TryGetValue(
                    tipoRegistro.IdentificadorRegistroPai,
                    out pai
                );
            }

            RegistroEdiNode registro = new()
            {
                NumeroLinha = numeroLinha,
                LinhaOriginal = linha,
                TipoRegistro = tipoRegistro,
                Pai = pai
            };

            foreach (Campo campo in tipoRegistro.Campos.OrderBy(campo => campo.Posicao))
            {
                string valor = ExtrairValorCampo(linha, campo);

                registro.Campos.Add(new CampoEdiValor
                {
                    CampoLayout = campo,
                    ValorOriginal = valor,
                    ValorAtual = valor
                });
            }

            return registro;
        }

        private string ExtrairValorCampo(string linha, Campo campo)
        {
            int indiceInicial = campo.Posicao - 1;

            if (indiceInicial >= linha.Length)
                return new string(' ', campo.Tamanho);

            int tamanhoDisponivel = linha.Length - indiceInicial;
            int tamanhoParaExtrair = Math.Min(campo.Tamanho, tamanhoDisponivel);

            string valor = linha.Substring(indiceInicial, tamanhoParaExtrair);

            return valor.PadRight(campo.Tamanho);
        }
    }
}