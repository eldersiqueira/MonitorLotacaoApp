using MonitorLotacaoApp.Models;
using Microsoft.Maui.Controls;
using System.Linq;
using System.Collections.Generic;

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
        // Linhas oficiais de Mogi das Cruzes e Linha de Teste alinhadas com o banco de dados
        var linhas = new List<LinhaOnibusModel>
        {
            new LinhaOnibusModel { CodigoLinha = "E108", NomeLinha = "Linha E108 - Jd. Universo" },
            new LinhaOnibusModel { CodigoLinha = "C603", NomeLinha = "Linha C603 - Estrada Imperial" },
            new LinhaOnibusModel { CodigoLinha = "C602", NomeLinha = "Linha C602 - Botujuru" },
            new LinhaOnibusModel { CodigoLinha = "E203", NomeLinha = "Linha E203 - Cj. Santo Ângelo" },
            new LinhaOnibusModel { CodigoLinha = "304",  NomeLinha = "Linha 304 - Socorro" },
            new LinhaOnibusModel { CodigoLinha = "TESTE", NomeLinha = "Linha Teste - Residência" }
        };

        CollectionLinhas.ItemsSource = linhas;
    }

    // Nome corrigido para coincidir exatamente com o SelectionChanged do XAML
    private async void OnLinhaSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var linhaSelecionada = e.CurrentSelection.FirstOrDefault() as LinhaOnibusModel;
        if (linhaSelecionada == null)
            return;

        CollectionLinhas.SelectedItem = null;

        // Navega passando tanto o código (para a API) quanto o nome (para exibição na tela)
        await Navigation.PushAsync(new ReportarLotacaoPage(linhaSelecionada.CodigoLinha, linhaSelecionada.NomeLinha));
    }
}