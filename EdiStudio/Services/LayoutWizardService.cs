using EdiStudio.Models;
using EdiStudio.ViewModels.LayoutWizard;
using EdiStudio.Views;

namespace EdiStudio.Services
{
    internal class LayoutWizardService : ILayoutWizardService
    {
        public Layout? CriarNovoLayout()
        {
            LayoutWizardViewModel viewModel = new();

            LayoutWizardWindow window = new() { DataContext = viewModel };

            bool? resultado = window.ShowDialog();

            if (resultado != true) 
                return null;

            return viewModel.LayoutCriado;
        }
    }
}
