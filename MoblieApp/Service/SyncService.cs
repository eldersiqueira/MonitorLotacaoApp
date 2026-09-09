using System.Net.Http.Json;
using MonitorLotacaoApp.Models;

namespace MonitorLotacaoApp.Services
{
    public static class SyncService
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://10.0.2.2/monitor-api/")
        };

        public static async Task TentarSincronizarRelatosPendentesAsync()
        {
            // Verifica se o dispositivo possui alguma conexão ativa com a rede
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                return; // Continua totalmente offline, mantendo os dados seguros no SQLite
            }

            try
            {
                // Busca todos os relatos que ainda não foram enviados ao servidor
                var relatosPendentes = await DatabaseService.ObterRelatosPendentesAsync();

                foreach (var relato in relatosPendentes)
                {
                    // Envia o relato via requisição POST para o endpoint da API backend
                    HttpResponseMessage resposta = await _httpClient.PostAsJsonAsync("relatos/sincronizar", relato);

                    if (resposta.IsSuccessStatusCode)
                    {
                        // Se o servidor confirmou o recebimento, atualiza o status local para sincronizado[cite: 1]
                        await DatabaseService.AtualizarRelatoSincronizadoAsync(relato);
                    }
                }
            }
            catch (Exception ex)
            {
                // Tratamento de exceção de rede para garantir resiliência mobile
                System.Diagnostics.Debug.WriteLine($"Erro na sincronização em segundo plano: {ex.Message}");
            }
        }
    }
}