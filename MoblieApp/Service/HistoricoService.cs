using System.Net.Http.Json;
using System.Text.Json;
using MonitorLotacaoApp.Models;

namespace MonitorLotacaoApp.Services
{
    public static class HistoricoService
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://10.0.2.2/monitor-api/")
        };

        public static async Task<List<HistoricoModel>> ObterHistoricoLinhaAsync(string codigoLinha)
        {
            try
            {
                HttpResponseMessage resposta = await _httpClient.GetAsync($"consultar_historico.php?linha={codigoLinha}");

                if (resposta.IsSuccessStatusCode)
                {
                    string jsonResponse = await resposta.Content.ReadAsStringAsync();

                    // Deserializa o retorno da API estruturada em PHP
                    var resultado = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

                    if (resultado.GetProperty("sucesso").GetBoolean())
                    {
                        var dadosArray = resultado.GetProperty("dados");
                        return JsonSerializer.Deserialize<List<HistoricoModel>>(dadosArray.GetRawText()) ?? new List<HistoricoModel>();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao buscar histórico: {ex.Message}");
            }

            return new List<HistoricoModel>();
        }
    }
}