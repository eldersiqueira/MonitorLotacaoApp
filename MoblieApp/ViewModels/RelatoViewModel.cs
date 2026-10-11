using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Devices.Sensors;
using MonitorLotacaoApp.Models;
using MonitorLotacaoApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonitorLotacaoApp.ViewModels
{
    public partial class RelatoViewModel : ObservableObject
    {
        [ObservableProperty]
        private string codigoLinha;

        [ObservableProperty]
        private string nivelLotacao; // Ex: "Vazio", "Moderado", "Lotado"

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string mensagem;

        [RelayCommand]
        public async Task EnviarRelatoAsync()
        {
            // 1. Validação básica de tela
            if (string.IsNullOrWhiteSpace(CodigoLinha) || string.IsNullOrWhiteSpace(NivelLotacao))
            {
                Mensagem = "Por favor, preencha a linha e o nível de lotação.";
                return;
            }

            IsBusy = true;
            Mensagem = "Validando a sua localização via GPS...";

            try
            {
                // 2. Simulação da rota oficial (Futuramente, isso virá da API do Google ou do MySQL)
                // Colocamos duas coordenadas de exemplo para formar um trajeto fictício
                var rotaOficial = new List<Location>
                {
                    new Location(-23.550520, -46.633308),
                    new Location(-23.551000, -46.634000)
                };

                // 3. Aciona a Cerca Geográfica (Geofencing) para cumprir a US 1.2
                bool estaNoTrajeto = await GeofencingService.ValidarPresencaNoTrajetoAsync(rotaOficial);

                if (!estaNoTrajeto)
                {
                    Mensagem = "Ação negada: Você precisa estar fisicamente no trajeto do ônibus para relatar a lotação.";
                    IsBusy = false;
                    return;
                }

                Mensagem = "Localização confirmada! Salvando relato...";

                // 4. Captura a coordenada exata de agora para registrar no banco de dados
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var localizacaoAtual = await Geolocation.Default.GetLocationAsync(request);

                // 5. Monta o objeto que o SQLite entende
                var novoRelato = new RelatoModel
                {
                    CodigoLinha = CodigoLinha,
                    NivelLotacao = NivelLotacao,
                    Latitude = localizacaoAtual?.Latitude ?? 0,
                    Longitude = localizacaoAtual?.Longitude ?? 0
                };

                // 6. Salva offline usando o seu DatabaseService
                await DatabaseService.SalvarRelatoOfflineAsync(novoRelato);

                Mensagem = "Relato salvo com sucesso no dispositivo! Ele será sincronizado em breve.";

                // Limpa os campos da tela para o próximo uso
                CodigoLinha = string.Empty;
                NivelLotacao = string.Empty;
            }
            catch (Exception ex)
            {
                Mensagem = $"Erro ao processar: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}