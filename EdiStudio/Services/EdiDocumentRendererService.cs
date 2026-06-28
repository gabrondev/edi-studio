using EdiStudio.Enums;
using EdiStudio.Models;
using EdiStudio.Models.Parsing;
using EdiStudio.Models.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace EdiStudio.Services
{
    internal class EdiDocumentRendererService : IEdiDocumentRendererService
    {
        private const int LarguraColunaLinha = 3;

        public DocumentoEdiRenderizado Renderizar(ArquivoEdiParseado arquivo)
        {
            DocumentoEdiRenderizado documento = new();

            int indice = 0;

            while (indice < arquivo.RegistrosEmOrdem.Count)
            {
                RegistroEdiNode primeiroRegistro = arquivo.RegistrosEmOrdem[indice];

                List<RegistroEdiNode> grupo = ObterGrupoSequencial(
                    arquivo.RegistrosEmOrdem,
                    indice
                );

                RenderizarGrupo(documento, grupo);

                indice += grupo.Count;
            }

            return documento;
        }

        private List<RegistroEdiNode> ObterGrupoSequencial(
            List<RegistroEdiNode> registros,
            int indiceInicial)
        {
            RegistroEdiNode primeiro = registros[indiceInicial];

            List<RegistroEdiNode> grupo = [];

            for (int i = indiceInicial; i < registros.Count; i++)
            {
                RegistroEdiNode atual = registros[i];

                bool mesmoTipo =
                    atual.TipoRegistro.Identificador == primeiro.TipoRegistro.Identificador;

                bool mesmoPai =
                    ReferenceEquals(atual.Pai, primeiro.Pai);

                if (!mesmoTipo || !mesmoPai)
                    break;

                grupo.Add(atual);
            }

            return grupo;
        }

        private void RenderizarGrupo(
            DocumentoEdiRenderizado documento,
            List<RegistroEdiNode> grupo)
        {
            if (grupo.Count == 0)
                return;

            RegistroEdiNode primeiro = grupo[0];
            TipoRegistro tipoRegistro = primeiro.TipoRegistro;

            if (documento.Linhas.Count > 0)
            {
                documento.Linhas.Add(new LinhaDocumentoEdi
                {
                    Texto = string.Empty,
                    TipoLinha = TipoLinhaDocumento.LinhaEmBranco
                });
            }

            documento.Linhas.Add(new LinhaDocumentoEdi
            {
                Texto = MontarTituloTabela(primeiro),
                TipoLinha = TipoLinhaDocumento.TituloTabela
            });

            List<Campo> campos = tipoRegistro.Campos
                .OrderBy(campo => campo.Posicao)
                .ToList();

            documento.Linhas.Add(new LinhaDocumentoEdi
            {
                Texto = MontarCabecalho(campos),
                TipoLinha = TipoLinhaDocumento.Cabecalho
            });

            documento.Linhas.Add(new LinhaDocumentoEdi
            {
                Texto = MontarSeparador(campos),
                TipoLinha = TipoLinhaDocumento.Separador
            });

            foreach (RegistroEdiNode registro in grupo)
            {
                documento.Linhas.Add(new LinhaDocumentoEdi
                {
                    Texto = MontarLinhaRegistro(registro, campos),
                    TipoLinha = TipoLinhaDocumento.Registro,
                    RegistroRelacionado = registro
                });
            }
        }

        private string MontarTituloTabela(RegistroEdiNode registro)
        {
            return $"{registro.TipoRegistro.Identificador} - {registro.TipoRegistro.Nome}";
        }

        private string MontarCabecalho(List<Campo> campos)
        {
            List<string> celulas =
            [
                RenderizarCelulaCabecalho("LIN", LarguraColunaLinha)
            ];

            foreach (Campo campo in campos)
            {
                celulas.Add(RenderizarCelulaCabecalho(campo.Nome, campo.Tamanho));
            }

            return string.Join("|", celulas);
        }

        private string MontarSeparador(List<Campo> campos)
        {
            List<string> celulas =
            [
                RenderizarCelulaSeparador(LarguraColunaLinha)
            ];

            foreach (Campo campo in campos)
            {
                celulas.Add(RenderizarCelulaSeparador(campo.Tamanho));
            }

            return string.Join("|", celulas);
        }

        private string MontarLinhaRegistro(
            RegistroEdiNode registro,
            List<Campo> campos)
        {
            List<string> celulas =
            [
                RenderizarCelula(
                    registro.NumeroLinha.ToString().PadLeft(LarguraColunaLinha),
                    LarguraColunaLinha
                )
            ];

            foreach (Campo campo in campos)
            {
                CampoEdiValor? campoValor = registro.Campos
                    .FirstOrDefault(valor => valor.CampoLayout == campo);

                string valor = campoValor?.ValorAtual
                    ?? new string(' ', campo.Tamanho);

                celulas.Add(RenderizarCelula(valor, campo.Tamanho));
            }

            return string.Join("|", celulas);
        }

        public string RenderizarLinhaRegistro(RegistroEdiNode registro)
        {
            List<Campo> campos = registro.TipoRegistro.Campos
                .OrderBy(campo => campo.Posicao)
                .ToList();

            return MontarLinhaRegistro(registro, campos);
        }

        private string RenderizarCelula(string valor, int largura)
        {
            string valorAjustado = valor.Length > largura
                ? valor[..largura]
                : valor.PadRight(largura);

            return $" {valorAjustado} ";
        }

        private string RenderizarCelulaCabecalho(string titulo, int largura)
        {
            string tituloAjustado = titulo.Length > largura
                ? titulo[..largura]
                : titulo.PadRight(largura);

            return $" {tituloAjustado} ";
        }

        private string RenderizarCelulaSeparador(int largura)
        {
            return $" {new string('-', largura)} ";
        }
    }
}
