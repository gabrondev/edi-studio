using EdiStudio.Enums;

namespace EdiStudio.Models
{
    /// <summary>
    /// Define a posição, o tamanho, o formato e a obrigatoriedade de um
    /// campo pertencente a um tipo de registro do layout EDI.
    /// </summary>
    /// <remarks>
    /// Esta classe descreve o campo no layout. O valor encontrado em um
    /// documento concreto é representado por
    /// <see cref="Parsing.CampoEdiValor"/>
    /// </remarks>
    public class Campo
    {
        /// <summary>
        /// Nome descritivo do campo
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Formato esperado para o valor do campo.
        /// </summary>
        public FormatoCampo Formato {  get; set; }

        /// <summary>
        /// Quantidade fixa de caracteres ocupada pelo campo.
        /// </summary>
        public int Tamanho { get; set; }

        /// <summary>
        /// Posição inicial do campo na linha EDI, utilizando índice iniciado em 1.
        /// </summary>
        public int Posicao { get; set; }

        /// <summary>
        /// Indica a obrigatoriedade do campo conforme o layout.
        /// </summary>
        public StatusCampo Status { get; set; }
    }
}
