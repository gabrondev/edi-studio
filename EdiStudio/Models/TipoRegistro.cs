namespace EdiStudio.Models
{
    /// <summary>
    /// Define a estrutura de um tipo de registro pertencente a um layout EDI.
    /// Contém o identificador, a relação hierárquica, o tamanho esperado e
    /// as definições dos campos que compõem o registro.
    /// </summary>
    /// <remarks>
    /// Esta classe representa metadados do layout, não uma ocorrência 
    /// concreta encontrada em um documento EDI.
    /// </remarks>
    public class TipoRegistro
    {
        /// <summary>
        /// Código de três caracteres utilizado para identificar o tipo 
        /// de registro no início de uma linha EDI.
        /// </summary>
        public string Identificador { get; set; } = string.Empty;

        /// <summary>
        /// Identificador do tipo de registro pai, quando o registro
        /// participa de uma estrutura hierárquica.
        /// </summary>
        public string? IdentificadorRegistroPai { get; set; }

        /// <summary>
        /// Nome descritivo do tipo de registro.
        /// </summary>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Quantidade máxima de ocorrências permitidas para cada
        /// registro pai.
        /// </summary>
        public int QtdeMaxOcorrenciasPorRegistroPai { get; set; }

        /// <summary>
        /// Tamanho total esperado para uma linha deste tipo de registro.
        /// </summary>
        public int TamanhoTipoRegistro {  get; set; }

        /// <summary>
        /// Definições dos campos que compõem este tipo de registro.
        /// </summary>
        public List<Campo> Campos { get; set; } = [];
    }
}