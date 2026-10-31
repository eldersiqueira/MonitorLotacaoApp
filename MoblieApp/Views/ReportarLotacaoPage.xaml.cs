using Microsoft.Maui.Devices.Sensors;
using MonitorLotacaoApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MonitorLotacaoApp.Views;

public partial class ReportarLotacaoPage : ContentPage
{
    private readonly string _codigoLinha;
    private readonly ApiService _apiService;
    private List<PontoItinerarioDto> _listaPontosLinha = new();

    public ReportarLotacaoPage(string codigoLinha, string nomeLinha)
    {
        InitializeComponent();
        _codigoLinha = codigoLinha;
        _apiService = new ApiService();

        if (lblNomeLinha != null)
            lblNomeLinha.Text = nomeLinha;

        CarregarItinerarioDaLinha();
    }

    private async void CarregarItinerarioDaLinha()
    {
        // Busca todos os pontos cadastrados no MySQL para esta linha específica
        var pontos = await _apiService.ObterItinerarioAsync(_codigoLinha);
        if (pontos != null && pontos.Count > 0)
        {
            _listaPontosLinha = pontos;
        }
        else if (_codigoLinha == "TESTE")
        {
            // Fallback exclusivo para a linha TESTE caso a API falhe na sua casa
            _listaPontosLinha = new List<PontoItinerarioDto>
            {
                new PontoItinerarioDto { nomePonto = "Casa (Teste)", latitude = -23.5394, longitude = -46.2167 }
            };
        }
    }

    private async Task<bool> ValidarPresencaNoPontoAsync()
    {
        if (_listaPontosLinha == null || _listaPontosLinha.Count == 0)
        {
            await DisplayAlert("Aguarde", "Carregando itinerário do servidor ou linha sem pontos cadastrados.", "OK");
            return false;
        }

        try
        {
            var request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(15));
            var location = await Geolocation.Default.GetLocationAsync(request);

            if (location != null)
            {
                bool estaNoTrajeto = false;
                double menorDistanciaKm = double.MaxValue;

                // Percorre todos os pontos da linha selecionada para verificar se o usuário está próximo de algum deles (Raio de 150m / 0.15km)
                foreach (var ponto in _listaPontosLinha)
                {
                    double distanciaKm = Location.CalculateDistance(
                        location.Latitude, location.Longitude,
                        ponto.latitude, ponto.longitude,
                        DistanceUnits.Kilometers);

                    if (distanciaKm < menorDistanciaKm)
                        menorDistanciaKm = distanciaKm;

                    if (distanciaKm <= 0.15)
                    {
                        estaNoTrajeto = true;
                        break;
                    }
                }

                if (estaNoTrajeto)
                {
                    return true;
                }

                // Se o usuário estiver longe de todos os pontos da linha escolhida, bloqueia o envio!
                await DisplayAlert("Fora de Alcance", $"Você está a {Math.Round(menorDistanciaKm * 1000, 0)} metros do trajeto da linha {_codigoLinha}. O reporte só é permitido próximo aos pontos oficiais.", "OK");
                return false;
            }
            else
            {
                await DisplayAlert("GPS Indisponível", "Não foi possível capturar a sua localização via satélite/GPS.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro de Sensor", $"Falha na geolocalização: {ex.Message}", "OK");
        }

        return false;
    }

    private async void OnVazioClicked(object sender, EventArgs e) => await ProcessarRelatorioAsync("Vazio");
    private async void OnModeradoClicked(object sender, EventArgs e) => await ProcessarRelatorioAsync("Moderado");
    private async void OnLotadoClicked(object sender, EventArgs e) => await ProcessarRelatorioAsync("Lotado");

    private async Task ProcessarRelatorioAsync(string nivelLotacao)
    {
        bool usuarioNoPonto = await ValidarPresencaNoPontoAsync();

        if (usuarioNoPonto)
            {
            
            bool inseridoMySQL = await _apiService.EnviarRelatoAsync(_codigoLinha, nivelLotacao);

            if (inseridoMySQL)
            {
                await DisplayAlert("Sucesso", $"Relato '{nivelLotacao}' enviado com sucesso para a linha {_codigoLinha}!", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Erro", "Falha ao gravar o relato no servidor MySQL.", "OK");
            }
        }
    }
}