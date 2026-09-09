using Microsoft.Maui.Devices.Sensors;
using MonitorLotacaoApp.Services;

namespace MonitorLotacaoApp.Views;

public partial class ReportarLotacaoPage : ContentPage
{
    private readonly string _codigoLinha;

    // Construtor que recebe a linha selecionada na Sprint 2 (US 2.1)
    public ReportarLotacaoPage(string codigoLinha)
    {
        InitializeComponent();
        _codigoLinha = codigoLinha;
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
            // Exibe indicador visual de carregamento ou processamento se necessário
            bool gpsDisponivel = await ValidarDispositivosGPSAsync();
            if (!gpsDisponivel) return;

            // Executa a validação de proximidade geográfica (Sprint 3)[cite: 1]
            bool estaNoTrajeto = await GeofencingService.ValidarPresencaNoTrajetoAsync();

            if (estaNoTrajeto)
            {
                await DisplayAlert("Sucesso", $"Relato de nível '{nivelLotacao}' para a linha {_codigoLinha} validado e computado com sucesso!", "OK");

                // Próximo passo técnico: Acionar o salvamento local via SQLite (US 1.3) caso esteja offline[cite: 1]
            }
            else
            {
                await DisplayAlert("Aviso de Rota", "Você está fora do trajeto monitorado desta linha de ônibus. O relato não pôde ser aceito.", "OK");
            }
        }
        catch (FeatureNotSupportedException)
        {
            await DisplayAlert("Compatibilidade", "A geolocalização não é suportada neste dispositivo.", "OK");
        }
        catch (PermissionException)
        {
            await DisplayAlert("Permissão Negada", "É necessário autorizar o acesso à localização para reportar a lotação.", "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro Inesperado", $"Ocorreu uma falha técnica: {ex.Message}", "OK");
        }
    }

    private async Task<bool> ValidarDispositivosGPSAsync()
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
}