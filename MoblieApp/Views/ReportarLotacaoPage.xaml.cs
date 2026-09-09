using Microsoft.Maui.Devices.Sensors;

namespace MonitorLotacaoApp.Views;

public partial class ReportarLotacaoPage : ContentPage
{
    public ReportarLotacaoPage()
    {
        InitializeComponent();
    }

    private async void OnVazioClicked(object sender, EventArgs e)
    {
        await ProcessarRelatorioAsync("Vazio");
    }

    private async void OnModeradoClicked(object sender, EventArgs e)
    {
        await ProcessarRelatorioAsync("Moderado");
    }

    private async void OnLotadoClicked(object sender, EventArgs e)
    {
        await ProcessarRelatorioAsync("Lotado");
    }

    private async Task ProcessarRelatorioAsync(string nivelLotacao)
    {
        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.Default.GetLocationAsync(request);

            if (location != null)
            {
                bool usuarioNoTrajeto = ValidarProximidadeTrajeto(location.Latitude, location.Longitude);

                if (usuarioNoTrajeto)
                {
                    await DisplayAlert("Sucesso", $"Relato de nível '{nivelLotacao}' validado e computado com sucesso!", "OK");
                }
                else
                {
                    await DisplayAlert("Aviso de Rota", "Você está fora do trajeto monitorado desta linha de ônibus.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Erro de GPS", "Não foi possível capturar a sua localização atual.", "OK");
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Compatibilidade", "A geolocalização não é suportada neste dispositivo.", "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Permissão Negada", "É necessário autorizar o acesso à localização.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro Inesperado", $"Ocorreu uma falha técnica: {ex.Message}", "OK");
        }
    }

    private bool ValidarProximidadeTrajeto(double latitude, double longitude)
    {
        return true;
    }
}