using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace MonitorLotacaoApp.Services
{
    public class GoogleMapsApiService
    {
        private readonly HttpClient _httpClient;

        // A tua chave da Google Cloud Console (restringida para Android/iOS)
        private const string ApiKey = "INSERE_A_TUA_CHAVE_AQUI";
        private const string BaseUrl = "https://maps.googleapis.com/maps/api/directions/json";

        public GoogleMapsApiService()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Consulta a Directions API para obter o percurso entre o passageiro e o destino da linha.
        /// </summary>
        public async Task<string> ObterRotaAsync(double latOrigem, double lonOrigem, double latDestino, double lonDestino)
        {
            try
            {
                // Constrói o URL de requisição via GET
                string requestUrl = $"{BaseUrl}?origin={latOrigem},{lonOrigem}&destination={latDestino},{lonDestino}&mode=driving&key={ApiKey}";

                var resposta = await _httpClient.GetAsync(requestUrl);

                if (resposta.IsSuccessStatusCode)
                {
                    // Devolve o JSON cru para depois extrairmos a Polyline (pontos da rota)
                    string jsonRetornado = await resposta.Content.ReadAsStringAsync();
                    return jsonRetornado;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Falha na comunicação com a Google Maps API: {ex.Message}");
                return null;
            }
        }
    }
}