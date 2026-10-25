using MonitorLotacaoApp.Services;

namespace MonitorLotacaoApp.Views;

public partial class ReportarLotacaoPage : ContentPage
{
    private readonly string _codigoLinha;
    private readonly ApiService _apiService;

    public ReportarLotacaoPage(string codigoLinha)
    {
        InitializeComponent();
        _codigoLinha = codigoLinha;
        _apiService = new ApiService();
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

    private async Task<bool> ValidarDispositivoGPSAsync()
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
                await DisplayAlert("Permissão Negada", "O aplicativo precisa da localização para validar o relato a bordo.", "OK");
                return false;
            }
            return true;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro de GPS", $"Não foi possível obter a geolocalização: {ex.Message}", "OK");
            return false;
        }
    }

    private async Task ProcessarRelatorioAsync(string nivelLotacao)
    {
        // 1. Validação de Geolocalização nativa do dispositivo
        bool gpsValido = await ValidarDispositivoGPSAsync();
        if (!gpsValido)
            return;

        // 2. Envio do Relato via API para o MySQL
        bool enviado = await _apiService.EnviarRelatoAsync(_codigoLinha, nivelLotacao);

        if (enviado)
        {
            await DisplayAlert("Sucesso", "Relato registado e sincronizado no servidor MySQL!", "OK");
            await Navigation.PopAsync();
        }
        else
        {
            await DisplayAlert("Erro de Conexão", "Não foi possível enviar o relato para o servidor central.", "OK");
        }
    }
}