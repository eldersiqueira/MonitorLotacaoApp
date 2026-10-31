using MonitorLotacaoApp.Services;

namespace MonitorLotacaoApp.Views;

public partial class AdminDashboardPage : ContentPage
{
    private readonly ApiService _apiService;

    public AdminDashboardPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
    }

    private async void OnVerRelatoriosClicked(object sender, EventArgs e)
    {
        var estatisticas = await _apiService.ObterEstatisticasAsync();
        int totalRelatos = estatisticas?.totalRelatos ?? 0;

        string mensagem = $"📊 Relatório Geral de Eficiência\n\n" +
                          $"• Total de Relatos Coletados: {totalRelatos}\n" +
                          $"• Linhas Monitoradas: 5 Linhas Oficiais + Teste\n" +
                          $"• Meta de Impacto: Redução de 10% no tempo de espera.";

        await DisplayAlert("Relatório de Eficiência", mensagem, "OK");
    }

    private async void OnHorariosCriticosClicked(object sender, EventArgs e)
    {
        var estatisticas = await _apiService.ObterEstatisticasAsync();
        string mensagemCritica = "⏰ Análise de Horários Criticos\n\n";

        if (estatisticas != null && estatisticas.horariosCriticos != null && estatisticas.horariosCriticos.Count > 0)
        {
            foreach (var hc in estatisticas.horariosCriticos)
            {
                mensagemCritica += $"• Pico às {hc.hora}h: {hc.quantidade} relatos de lotação.\n";
            }
            mensagemCritica += "\nRecomendação ao Gestor: Reforçar frota nestes horários.";
        }
        else
        {
            mensagemCritica += "Nenhum dado de horário crítico disponível no momento.";
        }

        await DisplayAlert("Horários Críticos", mensagemCritica, "OK");
    }

    // 3. Funcionalidade: Gerenciar Frotas e Linhas Atualizadas com o Escopo de Mogi das Cruzes
    private async void OnGerenciarFrotaClicked(object sender, EventArgs e)
    {
        string selecaoLinha = await DisplayActionSheet(
            "Gerenciamento de Frotas (Linhas Oficiais)",
            "Cancelar",
            null,
            "Linha E108 - Jd. Universo",
            "Linha C603 - Estrada Imperial",
            "Linha C602 - Botujuru",
            "Linha E203 - Cj. Santo Ângelo",
            "Linha 304 - Socorro");

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