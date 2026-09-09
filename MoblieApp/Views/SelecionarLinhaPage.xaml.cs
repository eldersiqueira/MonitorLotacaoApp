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
        // Mock inicial das 5 linhas municipais previstas no escopo do TCC
        var linhas = new List<LinhaOnibusModel>
        {
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

        // Limpa a seleção visual
        CollectionLinhas.SelectedItem = null;

        // Navega para a tela de Reporte de Lotação passando a linha escolhida
        await Navigation.PushAsync(new ReportarLotacaoPage());
    }
}