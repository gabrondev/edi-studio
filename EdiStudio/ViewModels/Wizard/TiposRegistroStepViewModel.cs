using EdiStudio.Commands;
using EdiStudio.Models;
using EdiStudio.ViewModels.LayoutWizard;
using System.Collections.ObjectModel;

namespace EdiStudio.ViewModels.Wizard
{
    public class TiposRegistroStepViewModel : WizardStepViewModelBase
    {
        private string _identificadorTipoRegistro = string.Empty;
        private string _nomeTipoRegistro = string.Empty;
        private string? _identificadorRegistroPai;
        private int _qtdeMaxOcorrenciasPorRegistroPai = 1;
        private int _tamanhoTipoRegistro = 250;

        public TiposRegistroStepViewModel(LayoutWizardState state) : base(state)
        {
            AdicionarTipoRegistroCommand = new RelayCommand(
                AdicionarTipoRegistro,
                PodeAdicionarTipoRegistro
            );
        }

        public override string Titulo => "Tipos de registro";

        public ObservableCollection<TipoRegistro> TiposRegistro => State.TiposRegistro;

        public string IdentificadorTipoRegistro
        {
            get => _identificadorTipoRegistro;
            set
            {
                _identificadorTipoRegistro = value;
                OnPropertyChanged();
                AdicionarTipoRegistroCommand.RaiseCanExecuteChanged();
            }
        }

        public string NomeTipoRegistro
        {
            get => _nomeTipoRegistro;
            set
            {
                _nomeTipoRegistro = value;
                OnPropertyChanged();
                AdicionarTipoRegistroCommand.RaiseCanExecuteChanged();
            }
        }

        public string? IdentificadorRegistroPai
        {
            get => _identificadorRegistroPai;
            set
            {
                _identificadorRegistroPai = value;
                OnPropertyChanged();
            }
        }

        public int QtdeMaxOcorrenciasPorRegistroPai
        {
            get => _qtdeMaxOcorrenciasPorRegistroPai;
            set
            {
                _qtdeMaxOcorrenciasPorRegistroPai = value;
                OnPropertyChanged();
            }
        }

        public int TamanhoTipoRegistro
        {
            get => _tamanhoTipoRegistro;
            set
            {
                _tamanhoTipoRegistro = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand AdicionarTipoRegistroCommand { get; }

        private void AdicionarTipoRegistro()
        {
            TipoRegistro tipoRegistro = new()
            {
                Identificador = IdentificadorTipoRegistro.Trim(),
                IdentificadorRegistroPai = string.IsNullOrWhiteSpace(IdentificadorRegistroPai)
                    ? null
                    : IdentificadorRegistroPai.Trim(),
                Nome = NomeTipoRegistro.Trim(),
                QtdeMaxOcorrenciasPorRegistroPai = QtdeMaxOcorrenciasPorRegistroPai,
                TamanhoTipoRegistro = TamanhoTipoRegistro
            };

            TiposRegistro.Add(tipoRegistro);

            OnPropertyChanged(nameof(TiposRegistro));

            IdentificadorTipoRegistro = string.Empty;
            IdentificadorRegistroPai = null;
            NomeTipoRegistro = string.Empty;
            QtdeMaxOcorrenciasPorRegistroPai = 1;
            TamanhoTipoRegistro = 250;
        }

        private bool PodeAdicionarTipoRegistro()
        {
            return IdentificadorValido(IdentificadorTipoRegistro) &&
                   !string.IsNullOrWhiteSpace(NomeTipoRegistro);
        }
        private bool IdentificadorValido(string? identificador)
        {
            if (string.IsNullOrWhiteSpace(identificador))
                return false;

            identificador = identificador.Trim();

            return identificador.Length == 3 &&
                   identificador.All(char.IsDigit);
        }

        public override bool PodeAvancar()
        {
            return TiposRegistro.Count > 0;
        }
    }
}
