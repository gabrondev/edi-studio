using EdiStudio.Enums;
using EdiStudio.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EdiStudio.ViewModels.LayoutWizard
{
    public class LayoutWizardState : BaseViewModel
    {
        private string _nomeLayout = string.Empty;
        public string NomeLayout
        {
            get => _nomeLayout;
            set
            {
                _nomeLayout = value;
                OnPropertyChanged();
            }
        }

        private TipoEdi _tipoEdiSelecionado = TipoEdi.OCOREN;
        public TipoEdi TipoEdiSelecionado
        {
            get => _tipoEdiSelecionado;
            set
            {
                _tipoEdiSelecionado = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TipoRegistro> TiposRegistro { get; } = [];
    }
}
