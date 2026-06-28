using EdiStudio.Enums;
using EdiStudio.Models.Parsing;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EdiStudio.Models.Rendering
{
    public class LinhaDocumentoEdi : INotifyPropertyChanged
    {
        private string _texto = string.Empty;
        public string Texto
        {
            get => _texto;
            set
            {
                if (_texto == value)
                    return;
                _texto = value;
                OnPropertyChanged();
            }
        }
        public TipoLinhaDocumento TipoLinha { get; set; }
        public RegistroEdiNode? RegistroRelacionado { get; set; }
        public bool EhSelecionavel => RegistroRelacionado is not null;
        
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(
            [CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
