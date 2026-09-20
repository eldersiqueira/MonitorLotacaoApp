using Microsoft.Maui.Devices.Sensors;

namespace MonitorLotacaoApp.Views;

public partial class ReportarLotacaoPage : ContentPage
{
    private readonly string _codigoLinha;

    // Mude para 'false' apenas quando for testar com GPS real na rua (Sprint 3)[cite: 2]
    private const bool MODO_SIMULACAO_GPS = true;

    public ReportarLotacaoPage(string codigoLinha)
    {
        InitializeComponent();
        _codigoLinha = codigoLinha;
        Title = $"Reportar Linha {_codigoLinha}";
    }

    private async void OnVazioClicked(object sender, EventArgs e) => await ProcessarEnvioRelato("Vazio");
    private async void OnModeradoClicked(object sender, EventArgs e) => await ProcessarEnvioRelato("Moderado");
    private async void OnLotadoClicked(object sender, EventArgs e) => await ProcessarEnvioRelato("Lotado");

    private async Task ProcessarEnvioRelato(string nivelLotacao)
    {
        Location localizacaoFinal = null;

        if (MODO_SIMULACAO_GPS)
        {
            // Simula uma coordenada válida para testes locais sem hardware de GPS
            localizacaoFinal = new Location(-23.550520, -46.633308);
            System.Diagnostics.Debug.WriteLine("Modo de Simulação GPS ativo para testes.");
        }
        else
        {
            try
            {
                var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted)
                {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permissão Negada", "A geolocalização é necessária para validar o trajeto[cite: 2].", "OK");
                    return;
                }

                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                localizacaoFinal = await Geolocation.GetLocationAsync(request);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Erro", $"Falha ao obter GPS: {ex.Message}", "OK");
                return;
            }
        }

        if (localizacaoFinal != null)
        {
            // Adiciona pontos de gamificação (US 5.2)[cite: 2]
            int pontos = Preferences.Get("PontosUsuario", 0) + 10;
            Preferences.Set("PontosUsuario", pontos);

            await DisplayAlert("Sucesso", $"Relato '{nivelLotacao}' enviado para a linha {_codigoLinha}!\nPontuação atual: {pontos} pts.", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Aviso", "Não foi possível validar a localização.", "OK");
        }
    }
}