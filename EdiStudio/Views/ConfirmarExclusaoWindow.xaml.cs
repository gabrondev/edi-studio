using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace EdiStudio.Views
{
    /// <summary>
    /// Interaction logic for ConfirmarExclusaoWindow.xaml
    /// </summary>
    public partial class ConfirmarExclusaoWindow : Window
    {
        public bool ExcluirSomenteSelecionado =>
            ExcluirSomenteSelecionadoCheckBox.IsChecked == true;

        public ConfirmarExclusaoWindow(
            int numeroLinha,
            int quantidadeDescendentes)
        {
            InitializeComponent();

            MensagemTextBlock.Text = quantidadeDescendentes == 0
                ? $"Deseja excluir a linha {numeroLinha}?"
                : $"A linha {numeroLinha} possui {quantidadeDescendentes} " +
                  "registro(s) descendente(s). Por padrão, todos serão excluídos.";

            ExcluirSomenteSelecionadoCheckBox.Visibility =
                quantidadeDescendentes > 0
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }

        private void Excluir_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
