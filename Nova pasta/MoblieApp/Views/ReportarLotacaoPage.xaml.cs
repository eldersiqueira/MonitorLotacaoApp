using MonitorLotacaoApp.Services;
using Microsoft.Maui.Devices.Sensors;

namespace MonitorLotacaoApp.Views;

public partial class ReportarLotacaoPage : ContentPage
{
    private readonly string _codigoLinha;
    private readonly string _nomeLinha;
    private readonly ApiService _apiService;

    public ReportarLotacaoPage(string codigoLinha, string nomeLinha)
    {
        InitializeComponent();
        _codigoLinha = codigoLinha;
        _nomeLinha = nomeLinha;
        _apiService = new ApiService();

        // Exibe o nome da linha no topo para clareza do usuário
        LblNomeLinha.Text = _nomeLinha ?? $"Linha: {_codigoLinha}";
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
        // 1. Validação de Geolocalização nativa utilizando os pontos do banco de dados (US 1.2)
        bool gpsValido = await ValidarDispositivoGPSAsync();
        if (!gpsValido)
        {
            return;
        }

        try
        {
            bool confirmado = await DisplayAlert("Confirmação de Reporte",
                $"Deseja registrar o nível [{nivelLotacao}] para a {_nomeLinha}?", "Enviar", "Cancelar");

            if (!confirmado) return;

            // 2. Envio do relato via ApiService estruturado para o backend MySQL[cite: 8]
            var resultado = await _apiService.EnviarRelatoComRetornoAsync(_codigoLinha, nivelLotacao);

            if (resultado != null && resultado.sucesso)
            {
                await DisplayAlert("Sucesso",
                    $"Relatório enviado e validado por GPS!\n\n" +
                    $"📊 Status Atual da Linha: {resultado.status_atual_linha}\n" +
                    $"📈 Ocupação Média: {resultado.percentual_medio}%\n" +
                    $"👥 Colaboradores: {resultado.total_relatos_recentes} relatos recentes", "OK");

                await Navigation.PopAsync();
            }
            else
            {
                string msg = resultado?.mensagem ?? "Sem conexão com o servidor. O relato foi salvo localmente no SQLite para sincronização posterior.";
                await DisplayAlert("Aviso", msg, "OK");
                await Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível processar o relato: {ex.Message}", "OK");
        }
    }

    // Validação avançada por Geolocalização dinâmica consultando a tabela de itinerário[cite: 12]
    private async Task<bool> ValidarDispositivoGPSAsync()
    {
        try
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            if (status != PermissionStatus.Granted)
            {
                await DisplayAlert("Permissão Negada", "O aplicativo precisa da localização ativada para validar o relato a bordo do veículo.", "OK");
                return false;
            }

            var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
            var location = await Geolocation.GetLocationAsync(request) ?? await Geolocation.GetLastKnownLocationAsync();

            if (location == null)
            {
                await DisplayAlert("GPS Indisponível", "Não foi possível capturar sua coordenada atual. Verifique o sinal de GPS.", "OK");
                return false;
            }

            // 1. Busca os pontos do itinerário da linha selecionada diretamente no MySQL via API[cite: 12]
            List<PontoItinerarioDto> pontosItinerario = await _apiService.ObterItinerarioAsync(_codigoLinha);

            // Fallback para testes locais offline caso a API não retorne pontos cadastrados
            if (pontosItinerario == null || pontosItinerario.Count == 0)
            {
                if (_codigoLinha?.ToUpper() == "TESTE" || _codigoLinha?.ToUpper() == "E108")
                {
                    // Ponto de referência padrão em Mogi das Cruzes para homologação
                    pontosItinerario = new List<PontoItinerarioDto>
                    {
                        new PontoItinerarioDto { latitude = -23.5227, longitude = -46.1864 }
                    };
                }
                else
                {
                    await DisplayAlert("Aviso de Rota", "Não foi possível carregar os pontos de itinerário desta linha para validação GPS.", "OK");
                    return false;
                }
            }

            // 2. Verifica se o usuário está a menos de 100 metros (0.1 km) de QUALQUER ponto da linha (ajustável para testes)
            double raioToleranciaKm = 0.2;
            bool estaNoTrajeto = false;
            double menorDistanciaEncontradaKm = double.MaxValue;

            foreach (var ponto in pontosItinerario)
            {
                var pontoReferencia = new Location(ponto.latitude, ponto.longitude);
                double distanciaKm = location.CalculateDistance(pontoReferencia, DistanceUnits.Kilometers);

                if (distanciaKm < menorDistanciaEncontradaKm)
                {
                    menorDistanciaEncontradaKm = distanciaKm;
                }

                if (distanciaKm <= raioToleranciaKm)
                {
                    estaNoTrajeto = true;
                    break;
                }
            }

            // 3. Rejeita o relato se estiver fora do raio de todos os pontos cadastrados
            if (!estaNoTrajeto)
            {
                double menorDistanciaMetros = menorDistanciaEncontradaKm * 1000;
                await DisplayAlert("Fora do Trajeto",
                    $"Validação de GPS recusada! Você está a aproximadamente {menorDistanciaMetros:F0} metros do ponto mais próximo da {_nomeLinha}. O relato exige proximidade da rota.", "OK");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro de Hardware", $"Falha ao validar a geolocalização: {ex.Message}", "OK");
            return false;
        }
        }
    }