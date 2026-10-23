using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MonitorLotacaoApp.ViewModels
{
    public partial class LinhaModel
    {
        public int Id { get; set; }
        public string NumeroENome { get; set; }
        public string Destino { get; set; }
    }

    public partial class SelecaoLinhaViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<LinhaModel> linhasDeOnibus;

        [ObservableProperty]
        private bool isBusy;

        public SelecaoLinhaViewModel()
        {
            LinhasDeOnibus = new ObservableCollection<LinhaModel>();
            CarregarLinhas();
        }

        private void CarregarLinhas()
        {
            IsBusy = true;

            // Simulação de carregamento de linhas oficiais (posteriormente integrado com a API PHP/MySQL ou SQLite)
            LinhasDeOnibus.Add(new LinhaModel { Id = 1, NumeroENome = "C192 - Vila Municipal", Destino = "Terminal Central" });
            LinhasDeOnibus.Add(new LinhaModel { Id = 2, NumeroENome = "E305 - Jundiapeba", Destino = "Estação CPTM" });
            LinhasDeOnibus.Add(new LinhaModel { Id = 3, NumeroENome = "C204 - Mogilar", Destino = "Shopping / Vila Mogilar" });

            IsBusy = false;
        }

        [RelayCommand]
        public async Task SelecionarLinhaAsync(LinhaModel linhaSelecionada)
        {
            if (linhaSelecionada == null) return;

            // Navega para a tela de relato, passando o ID da linha selecionada via parâmetro de rota
            await Shell.Current.GoToAsync($"RelatoPage?linhaId={linhaSelecionada.Id}");
        }
    }
}