using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonitorLotacaoApp.Services;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace MonitorLotacaoApp.ViewModels
{
    [QueryProperty(nameof(LinhaId), "linhaId")]
    public partial class RelatoViewModel : ObservableObject
    {
        [ObservableProperty]
        private int linhaId;

        [ObservableProperty]
        private string statusGPS = "Verificando proximidade da linha via GPS...";

        [ObservableProperty]
        private string mensagemRetorno;

        [ObservableProperty]
        private bool isBusy;

        public RelatoViewModel()
        {
            ValidarLocalizacaoInicial();
        }

        private async void ValidarLocalizacaoInicial()
        {
            // Executa a validação por geolocalização (US 1.2)
            bool estaNoTrajeto = await GeofencingService.ValidarPresencaNoTrajetoAsync();

            if (estaNoTrajeto)
            {
                StatusGPS = "📍 Localização confirmada no trajeto. Pode enviar o seu relato!";
            }
            else
            {
                StatusGPS = "⚠️ Aviso: Você parece estar fora da rota oficial, mas o relato será salvo offline.";
            }
        }

        [RelayCommand]
        public async Task EnviarRelatoAsync(string nivelLotacao)
        {
            if (string.IsNullOrEmpty(nivelLotacao)) return;

            IsBusy = true;
            MensagemRetorno = string.Empty;

            try
            {
                // Validação final de GPS antes de persistir
                bool dentroDaArea = await GeofencingService.ValidarPresencaNoTrajetoAsync();

                // Aqui entra a persistência Local (SQLite - Offline First) ou envio direto para a API PHP
                await Task.Delay(1000); // Simulando requisição de rede ou escrita no SQLite

                MensagemRetorno = $"Relato '{nivelLotacao}' enviado com sucesso para a linha {LinhaId}!";

                // Aguarda um instante e retorna para a seleção de linha
                await Task.Delay(1500);
                await Shell.Current.GoToAsync("..");
            }
            catch (System.Exception ex)
            {
                MensagemRetorno = $"Erro ao enviar relato: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}