using System.Text.Json;
using MonitorLotacaoApp.Models;

namespace MonitorLotacaoApp.Service
{
    public class RelatorioService
    {
        private readonly HttpClient _httpClient;

        public RelatorioService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<RelatorioModel>> ObterRelatorioAsync()
        {
            try
            {
                // ATENÇÃO: Use '10.0.2.2' para emulador Android ou o IP local da sua máquina (ex: '192.168.1.15') para celular físico.
                string url = "http://10.0.2.2/monitor-api/gerar_relatorio.php";

                var resposta = await _httpClient.GetAsync(url);
                if (resposta.IsSuccessStatusCode)
                {
                    var jsonContent = await resposta.Content.ReadAsStringAsync();
                    var resultado = JsonSerializer.Deserialize<RelatorioResponseDto>(jsonContent);
                    return resultado?.DadosEstatisticos ?? new List<RelatorioModel>();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao buscar relatório: {ex.Message}");
            }

            return new List<RelatorioModel>();
        }
    }
}