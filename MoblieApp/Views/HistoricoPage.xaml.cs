using MonitorLotacaoApp.Services;

namespace MonitorLotacaoApp.Views;

public partial class HistoricoPage : ContentPage
{
    private readonly string _codigoLinha;

    public HistoricoPage(string codigoLinha)
    {
        InitializeComponent();
        _codigoLinha = codigoLinha;
        CarregarDadosHistorico();
    }

    private async void CarregarDadosHistorico()
    {
        // Busca os dados históricos através do serviço que se comunica com a API PHP
        var historico = await HistoricoService.ObterHistoricoLinhaAsync(_codigoLinha);

        // Atribui a lista ao CollectionView da interface
        CollectionHistorico.ItemsSource = historico;
    }
}