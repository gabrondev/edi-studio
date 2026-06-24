using EdiStudio.ViewModels;
using System.Windows;

namespace EdiStudio
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainWindowViewModel();
        }
    }
}