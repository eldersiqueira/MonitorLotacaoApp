namespace MonitorLotacaoApp.Views;

public partial class PassageiroHomePage : ContentPage
{
    public PassageiroHomePage()
    {
        InitializeComponent();
    }

    // US 1.1 - Interface Simplificada de Toque Único (Verde)[cite: 19]
    private async void OnReportarVazioClicked(object sender, EventArgs e)
    {
        // Na Sprint 3, aqui entrará a validação de Geolocalização (US 1.2) e o SQLite (US 1.3)
        await DisplayAlert("Sucesso", "Lotação 'Vazio / Confortável' registrada localmente.", "OK");
    }

    // US 1.1 - Interface Simplificada de Toque Único (Amarelo)[cite: 19]
    private async void OnReportarModeradoClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Sucesso", "Lotação 'Moderado / Em Pé' registrada localmente.", "OK");
    }

    // US 1.1 - Interface Simplificada de Toque Único (Vermelho)[cite: 19]
    private async void OnReportarLotadoClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Sucesso", "Lotação 'Lotado / Cheio' registrada localmente.", "OK");
    }

    // Módulo 2 - Consultar Média de Lotação por Horário[cite: 21]
    private async void OnConsultarHistoricoClicked(object sender, EventArgs e)
    {
        // Funcionalidade prevista para a Sprint 4 (Consultas e Indicadores)[cite: 23]
        await DisplayAlert("Em desenvolvimento", "A consulta de média por horário será implementada na Sprint 4.", "OK");
    }

    // Resolve o erro MAUIX2002 apontado na linha 66[cite: 13]
    private async void OnSwitchProfileClicked(object sender, EventArgs e)
    {
        // Limpa a pilha e redireciona para a tela de Login
        await Shell.Current.GoToAsync("//LoginPage");
    }
}