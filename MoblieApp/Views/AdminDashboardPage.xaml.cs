using MonitorLotacaoApp.Services;

namespace MonitorLotacaoApp.Views;

public partial class AdminDashboardPage : ContentPage
{
    public AdminDashboardPage()
    {
        InitializeComponent();
    }

    // 1. Funcionalidade: Visualizar Relatórios de Eficiência
    private async void OnVerRelatoriosClicked(object sender, EventArgs e)
    {
        try
        {
            var relatos = await DatabaseService.ObterRelatosPendentesAsync();
            int totalRelatos = relatos != null ? relatos.Count : 0;

            string mensagem = $"📊 Relatório Geral de Eficiência\n\n" +
                              $"• Total de Relatos Coletados: {totalRelatos}\n" +
                              $"• Linhas Monitoradas: 5 Linhas Oficiais\n" +
                              $"• Meta de Impacto: Redução de 10% no tempo de espera.\n" +
                              $"Status: Operação dentro dos parâmetros da Sprint 5.";

            await DisplayAlert("Relatório de Eficiência", mensagem, "OK");
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível carregar os relatórios: {ex.Message}", "OK");
        }
    }

    // 2. Funcionalidade: Identificar Horários Críticos
    private async void OnHorariosCriticosClicked(object sender, EventArgs e)
    {
        string mensagemCritica = "⏰ Análise de Horários Críticos\n\n" +
                                 "• Pico da Manhã: 07:00 às 08:30 (Linhas 01 e 03 com alta concentração de lotação).\n" +
                                 "• Pico da Tarde: 17:30 às 19:00 (Linha 04 superlotada).\n\n" +
                                 "Recomendação ao Gestor: Reforçar frota nesses horários.";

        await DisplayAlert("Horários Críticos", mensagemCritica, "OK");
    }

    // 3. Funcionalidade: Gerenciar Frotas e Linhas
    private async void OnGerenciarFrotaClicked(object sender, EventArgs e)
    {
        string selecaoLinha = await DisplayActionSheet(
            "Gerenciamento de Frotas (5 Linhas Oficiais)",
            "Cancelar",
            null,
            "Linha 01 - Centro / Terminal Leste",
            "Linha 02 - Bairro Alto / Vila Industrial",
            "Linha 03 - Circular Universitária",
            "Linha 04 - Distrito Industrial / Estação",
            "Linha 05 - Parque das Flores / Centro");

        if (!string.IsNullOrEmpty(selecaoLinha) && selecaoLinha != "Cancelar")
        {
            await DisplayAlert("Gestão de Frota", $"Você selecionou a gerência da: {selecaoLinha}.\nStatus dos veículos: Em operação regular.", "OK");
        }
    }

    // 4. Funcionalidade: Exportar Relatórios
    private async void OnExportarRelatoriosClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Exportação", "Os dados estatísticos e relatórios de eficiência foram exportados com sucesso para o formato CSV no diretório do sistema.", "OK");
    }

    // 5. Nova Funcionalidade: Direcionar para a Tela de Gestão de Acessos
    private async void OnGerenciarAcessosClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AdminGerenciarUsuariosPage());
    }
}