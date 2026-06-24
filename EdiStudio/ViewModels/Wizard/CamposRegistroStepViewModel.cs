using EdiStudio.Commands;
using EdiStudio.Enums;
using EdiStudio.Models;
using EdiStudio.ViewModels.LayoutWizard;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace EdiStudio.ViewModels.Wizard
{
    public class CamposRegistroStepViewModel : WizardStepViewModelBase
    {
        private TipoRegistro? _tipoRegistroSelecionado;
        private string _nomeCampo = string.Empty;
        private FormatoCampo _formatoCampoSelecionado = FormatoCampo.Alfanumerico;
        private int _tamanhoCampo = 1;
        private int _posicaoCampo = 1;
        private StatusCampo _statusCampoSelecionado = StatusCampo.Obrigatorio; 
        public CamposRegistroStepViewModel(LayoutWizardState state) : base(state)
        {
            AdicionarCampoCommand = new RelayCommand(AdicionarCampo, PodeAdicionarCampo);
        }

        public override string Titulo => "Campos dos registros";
        public ObservableCollection<TipoRegistro> TiposRegistro => State.TiposRegistro;
        public ObservableCollection<Campo> CamposDoRegistroSelecionado { get; } = [];
        public ObservableCollection<FormatoCampo> FormatosCampoDisponiveis { get; } =
            new()
            {
                FormatoCampo.Alfanumerico,
                FormatoCampo.Numerico
            };
        public ObservableCollection<StatusCampo> StatusCampoDisponiveis { get; } =
            new()
            {
                StatusCampo.Condicional,
                StatusCampo.Obrigatorio
            };
        public TipoRegistro? TipoRegistroSelecionado
        {
            get => _tipoRegistroSelecionado;
            set
            {
                _tipoRegistroSelecionado = value;
                OnPropertyChanged();

                CarregarCamposDoRegistroSelecionado();
                AdicionarCampoCommand.RaiseCanExecuteChanged();
            }
        }
        public string NomeCampo
        {
            get => _nomeCampo;
            set
            {
                _nomeCampo = value;
                OnPropertyChanged();

                AdicionarCampoCommand.RaiseCanExecuteChanged();
            }
        }
        public FormatoCampo FormatoCampoSelecionado
        {
            get => _formatoCampoSelecionado;
            set
            {
                _formatoCampoSelecionado= value;
                OnPropertyChanged();
            }
        }
        public int TamanhoCampo
        {
            get => _tamanhoCampo;
            set
            {
                _tamanhoCampo = value;
                OnPropertyChanged();

                AdicionarCampoCommand.RaiseCanExecuteChanged();
            }
        }
        public int PosicaoCampo
        {
            get => _posicaoCampo;
            set
            {
                _posicaoCampo = value;
                OnPropertyChanged();

                AdicionarCampoCommand.RaiseCanExecuteChanged();
            }
        }

        public StatusCampo StatusCampoSelecionado
        {
            get => _statusCampoSelecionado;
            set
            {
                _statusCampoSelecionado = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand AdicionarCampoCommand { get; }

        private void AdicionarCampo()
        {
            if (TipoRegistroSelecionado is null)
                return;

            Campo campo = new()
            {
                Nome = NomeCampo.Trim(),
                Formato = FormatoCampoSelecionado,
                Tamanho = TamanhoCampo,
                Posicao = PosicaoCampo,
                Status = StatusCampoSelecionado
            };

            TipoRegistroSelecionado.Campos.Add(campo);
            CamposDoRegistroSelecionado.Add(campo);

            NomeCampo = string.Empty;
            FormatoCampoSelecionado = FormatoCampo.Alfanumerico;
            TamanhoCampo = 1;
            PosicaoCampo = 1;
            StatusCampoSelecionado = StatusCampo.Obrigatorio;
        }

        private bool PodeAdicionarCampo()
        {
            return TipoRegistroSelecionado is not null &&
                !string.IsNullOrWhiteSpace(NomeCampo) &&
                TamanhoCampo > 0 &&
                PosicaoCampo > 0;
        }

        private void CarregarCamposDoRegistroSelecionado()
        {
            CamposDoRegistroSelecionado.Clear();

            if (TipoRegistroSelecionado is null)
                return;

            foreach(Campo campo in TipoRegistroSelecionado.Campos)
            {
                CamposDoRegistroSelecionado.Add(campo);
            }
        }
    }
}
