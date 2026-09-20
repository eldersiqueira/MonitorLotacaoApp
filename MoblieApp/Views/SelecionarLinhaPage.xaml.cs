using MonitorLotacaoApp.Models;

namespace MonitorLotacaoApp.Views;

public partial class SelecionarLinhaPage : ContentPage
{
    public SelecionarLinhaPage()
    {
        InitializeComponent();
        CarregarLinhasDeOnibus();
    }

    private void CarregarLinhasDeOnibus()
    {
        var linhas = new List<LinhaOnibusModel>
        {
            new LinhaOnibusModel { CodigoLinha = "06", NomeLinha = "Linha 06 - Estudantes / Centro (Mogi)", Trajeto = "Via Rodovia Prof. Alfredo Rolim de Moura" },
            new LinhaOnibusModel { CodigoLinha = "01", NomeLinha = "Linha 01 - Centro / Terminal Leste", Trajeto = "Via Av. Principal" },
            new LinhaOnibusModel { CodigoLinha = "02", NomeLinha = "Linha 02 - Bairro Alto / Vila Industrial", Trajeto = "Via Rodovia Municipal" },
            new LinhaOnibusModel { CodigoLinha = "03", NomeLinha = "Linha 03 - Circular Universitária", Trajeto = "Via Campus / Hospital" },
            new LinhaOnibusModel { CodigoLinha = "04", NomeLinha = "Linha 04 - Distrito Industrial / Estação", Trajeto = "Via Av. dos Trabalhadores" },
            new LinhaOnibusModel { CodigoLinha = "05", NomeLinha = "Linha 05 - Parque das Flores / Centro", Trajeto = "Via Bairro Comercial" }
            
        };

        CollectionLinhas.ItemsSource = linhas;
    }

    private async void OnLinhaSelected(object sender, SelectionChangedEventArgs e)
    {
        var linhaSelecionada = e.CurrentSelection.FirstOrDefault() as LinhaOnibusModel;
        if (linhaSelecionada == null)
            return;

        // Limpa a seleção visual para permitir novos cliques na mesma linha
        CollectionLinhas.SelectedItem = null;

        // Executa a navegação na thread principal garantindo a abertura da tela de reporte (US 2.1 e US 2.2)[cite: 2]
        await MainThread.InvokeOnMainThreadAsync(async () =>
        {
            await Navigation.PushAsync(new ReportarLotacaoPage(linhaSelecionada.CodigoLinha));
        });
    }
}