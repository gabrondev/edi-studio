using EdiStudio.ViewModels.LayoutWizard;
using System.Windows;

namespace EdiStudio.Views
{
    /// <summary>
    /// Interaction logic for LayoutWizardWindow.xaml
    /// </summary>
    public partial class LayoutWizardWindow : Window
    {
        public LayoutWizardWindow()
        {
            InitializeComponent();

            DataContextChanged += LayoutWizardWindow_DataContextChanged;
        }

        private void LayoutWizardWindow_DataContextChanged(
            object sender,
            DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is LayoutWizardViewModel oldViewModel)
            {
                oldViewModel.SolicitarFechamento -= ViewModel_SolicitarFechamento;
            }

            if (e.NewValue is LayoutWizardViewModel newViewModel)
            {
                newViewModel.SolicitarFechamento += ViewModel_SolicitarFechamento;
            }
        }

        private void ViewModel_SolicitarFechamento(bool? resultado)
        {
            DialogResult = resultado;
            Close();
        }
    }
}
