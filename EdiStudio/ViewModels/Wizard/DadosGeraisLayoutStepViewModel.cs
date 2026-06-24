using EdiStudio.Enums;
using EdiStudio.ViewModels.LayoutWizard;
using System.Collections.ObjectModel;

namespace EdiStudio.ViewModels.Wizard
{
    public class DadosGeraisLayoutStepViewModel : WizardStepViewModelBase
    {
        public DadosGeraisLayoutStepViewModel(LayoutWizardState state) : base(state) { }
        
        public override string Titulo => "Dados gerais do layout";

        public string NomeLayout
        {
            get => State.NomeLayout;
            set
            {
                State.NomeLayout = value;
                OnPropertyChanged();
            }
        }

        public TipoEdi TipoEdiSelecionado
        {
            get => State.TipoEdiSelecionado;
            set
            {
                State.TipoEdiSelecionado = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TipoEdi> TiposEdiDisponiveis { get; } =
            new()
            {
                TipoEdi.OCOREN,
                TipoEdi.CONEMB,
                TipoEdi.DOCCOB,
                TipoEdi.NOTFIS
            };

        public override bool PodeAvancar()
        {
            return !string.IsNullOrWhiteSpace(NomeLayout);
        }
    }
}
