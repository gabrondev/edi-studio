using EdiStudio.Models;
using EdiStudio.Models.Parsing;

namespace EdiStudio.Tests.Models.Parsing
{
    public class CampoEdiValorTests
    {
        [Test]
        public void ValorEditavel_QuandoMenorQueTamanho_PreencheComEspacos()
        {
            CampoEdiValor campoValor = new()
            {
                CampoLayout = new Campo { Tamanho = 5 }
            };

            campoValor.ValorEditavel = "AB";

            Assert.That(campoValor.ValorAtual, Is.EqualTo("AB   "));
        }

        [Test]
        public void ValorEditavel_QuandoMaiorQueTamanho_TruncaValor()
        {
            CampoEdiValor campoValor = new()
            {
                CampoLayout = new Campo() { Tamanho = 5}
            };

            campoValor.ValorEditavel = "123456";

            Assert.That(campoValor.ValorAtual, Is.EqualTo("12345"));
        }

        [Test]
        public void ValorEditavel_QuandoLido_RemoveEspacosFinais()
        {
            CampoEdiValor campoValor = new()
            {
                CampoLayout = new Campo { Tamanho = 5},
                ValorAtual = "AB   "
            };

            string resultado = campoValor.ValorEditavel;

            Assert.That(resultado, Is.EqualTo("AB"));
        }

        [Test]
        public void ConfirmarValorAtualComoOriginal_RemoveIndicadorDeAlteracao()
        {
            CampoEdiValor campoValor = new()
            {
                CampoLayout = new Campo { Tamanho = 5 },
                ValorOriginal = "AB   ",
                ValorAtual = "AB   "
            };

            campoValor.ValorEditavel = "XYZ";

            Assert.That(campoValor.FoiAlterado, Is.True);

            campoValor.ConfirmarValorAtualComoOriginal();

            Assert.That(campoValor.FoiAlterado, Is.False);
            Assert.That(campoValor.ValorOriginal, Is.EqualTo("XYZ  "));
        }
    }
}
