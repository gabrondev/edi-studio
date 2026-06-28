using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EdiStudio.Models.Parsing
{
    public class CampoEdiValor : INotifyPropertyChanged
    {
        private string _valorAtual = string.Empty;
        public Campo CampoLayout { get; set; } = new();
        public string ValorOriginal { get; set; } = string.Empty;
        public string ValorAtual
        {
            get => _valorAtual;
            set
            {
                string valorAjustado = AjustarValorAoTamanho(value);

                if (_valorAtual == valorAjustado)
                    return;

                _valorAtual = valorAjustado;

                OnPropertyChanged();
                OnPropertyChanged(nameof(ValorEditavel));
                OnPropertyChanged(nameof(FoiAlterado));
            }
        }
        public string ValorEditavel
        {
            get => ValorAtual.TrimEnd();
            set => ValorAtual = value;
        }
        public bool FoiAlterado => ValorAtual != ValorOriginal;
        public event PropertyChangedEventHandler? PropertyChanged;

        private string AjustarValorAoTamanho(string? valor)
        {
            valor ??= string.Empty;

            int tamanho = CampoLayout.Tamanho;

            if (tamanho <= 0)
                return valor;

            if (valor.Length > tamanho)
                return valor[..tamanho];

            return valor.PadRight(tamanho);

        }

        public void ConfirmarValorAtualComoOriginal()
        {
            ValorOriginal = ValorAtual;

            OnPropertyChanged(nameof(ValorOriginal));
            OnPropertyChanged(nameof(FoiAlterado));
        }

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
