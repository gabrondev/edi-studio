using EdiStudio.ViewModels.LayoutWizard;

namespace EdiStudio.ViewModels.Wizard
{
    public abstract class WizardStepViewModelBase : BaseViewModel
    {
        protected WizardStepViewModelBase(LayoutWizardState state)
        {
            State = state;
        }

        protected LayoutWizardState State { get; }
        public abstract string Titulo { get; }
        public virtual bool PodeAvancar()
        {
            return true;
        }
    }
}
