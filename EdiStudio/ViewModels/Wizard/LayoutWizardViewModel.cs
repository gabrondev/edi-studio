using EdiStudio.Commands;
using EdiStudio.Models;
using EdiStudio.ViewModels.Wizard;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace EdiStudio.ViewModels.LayoutWizard
{
    internal class LayoutWizardViewModel : BaseViewModel
    {
        private readonly LayoutWizardState _state = new();
        private int _indiceEtapaAtual;
        private WizardStepViewModelBase _etapaAtualViewModel = null!;

        public LayoutWizardViewModel()
        {
            Etapas =
                [
                    new DadosGeraisLayoutStepViewModel(_state),
                    new TiposRegistroStepViewModel(_state),
                    new CamposRegistroStepViewModel(_state)
                ];

            ProximoCommand = new RelayCommand(Proximo, PodeAvancar);
            VoltarCommand = new RelayCommand(Voltar, PodeVoltar);

            EtapaAtualViewModel = Etapas[0];
        }

        public ObservableCollection<WizardStepViewModelBase> Etapas { get; }

        public WizardStepViewModelBase EtapaAtualViewModel
        {
            get => _etapaAtualViewModel;
            set
            {
                if (_etapaAtualViewModel is not null)
                    _etapaAtualViewModel.PropertyChanged -= EtapaAtualViewModel_PropertyChanged;

                _etapaAtualViewModel = value;

                _etapaAtualViewModel.PropertyChanged += EtapaAtualViewModel_PropertyChanged;

                OnPropertyChanged();
                OnPropertyChanged(nameof(TituloEtapa));
                OnPropertyChanged(nameof(IndicadorEtapa));
                OnPropertyChanged(nameof(TextoBotaoProximo));

                VoltarCommand.RaiseCanExecuteChanged();
                ProximoCommand.RaiseCanExecuteChanged();
            }
        }

        public int TotalEtapas => Etapas.Count;
        public int NumeroEtapaAtual => _indiceEtapaAtual + 1;
        public string IndicadorEtapa => $"Etapa {NumeroEtapaAtual} de {TotalEtapas}";

        public string TituloEtapa => EtapaAtualViewModel.Titulo;

        public string TextoBotaoProximo => NumeroEtapaAtual == TotalEtapas ? "Finalizar" : "Próximo";

        public RelayCommand ProximoCommand { get; }
        public RelayCommand VoltarCommand { get; }
        public event Action<bool?>? SolicitarFechamento;

        private void EtapaAtualViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            ProximoCommand.RaiseCanExecuteChanged();
        }

        public Layout? LayoutCriado { get; private set; }

        private void Proximo()
        {
            if (_indiceEtapaAtual < Etapas.Count - 1)
            {
                _indiceEtapaAtual++;
                EtapaAtualViewModel = Etapas[_indiceEtapaAtual];
                return;
            }

            Finalizar();
        }

        private bool PodeAvancar()
        {
            return EtapaAtualViewModel.PodeAvancar();
        }

        private void Voltar()
        {
            if (_indiceEtapaAtual <= 0)
                return;

            _indiceEtapaAtual--;
            EtapaAtualViewModel = Etapas[_indiceEtapaAtual];
        }

        private bool PodeVoltar()
        {
            return _indiceEtapaAtual > 0;
        }

        private void Finalizar()
        {
            LayoutCriado = new Layout
            {
                Nome = _state.NomeLayout,
                TipoEdi = _state.TipoEdiSelecionado,
                TiposRegistro = _state.TiposRegistro.ToList()
            };

            SolicitarFechamento?.Invoke(true);
        }
    }
}
