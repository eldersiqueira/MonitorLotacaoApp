using MonitorLotacaoApp.Service;
using MonitorLotacaoApp.Models;

namespace MonitorLotacaoApp.Views;

public partial class RelatoriosPage : ContentPage
{
    private readonly RelatorioService _relatorioService;

    public RelatoriosPage()
    {
        InitializeComponent();
        _relatorioService = new RelatorioService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var dados = await _relatorioService.ObterRelatorioAsync();
        CollectionRelatorio.ItemsSource = dados;
    }
}