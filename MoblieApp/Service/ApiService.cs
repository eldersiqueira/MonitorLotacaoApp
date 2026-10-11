using System.Text.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System;

namespace MonitorLotacaoApp.Services
{
    // 1. Classe DTO (Data Transfer Object) para espelhar o retorno JSON do seu PHP
    public class LoginResponse
    {
        public bool sucesso { get; set; }
        public string mensagem { get; set; }
        public int? tipo_usuario { get; set; }
        public string nome { get; set; }
    }

    public class ApiService
    {
        private readonly HttpClient _httpClient;

        // Se estiver testando no Emulador Android via XAMPP local, o IP padrão é 10.0.2.2
        // Troque o caminho /api_tcc_onibus/ para o nome exato da pasta no seu htdocs
        private const string BaseUrl = "http://10.0.2.2/monitor-api/";

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> RealizarLoginRealAsync(string email, string senha)
        {
            try
            {
                // Alterado para enviar os dados de forma compatível com o $_POST do PHP
                var parametros = new Dictionary<string, string>
        {
            { "email", email },
            { "senha", senha }
        };
                var conteudo = new FormUrlEncodedContent(parametros);

                var resposta = await _httpClient.PostAsync($"{BaseUrl}login.php", conteudo);

                if (resposta.IsSuccessStatusCode)
                {
                    // O PHP retorna um JSON, por exemplo: {"sucesso":true, "tipo_usuario":0, "nome":"Utilizador"}
                    string retornoJson = await resposta.Content.ReadAsStringAsync();
                    return retornoJson;
                }
                return "ErroDeConexao";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na API: {ex.Message}");
                return "Falha";
            }
        }
    }
}