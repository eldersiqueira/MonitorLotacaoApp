using System.Text;
using System.Text.Json;

namespace MonitorLotacaoApp.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        // Coloque aqui o IP do seu servidor local (XAMPP/Tomcat) ou domínio real
        // Exemplo para emulador Android acessando localhost: "http://10.0.2.2/api/"
        private const string BaseUrl = "http://SEU_IP_OU_DOMINIO/api/";

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        // Método real para autenticar conectando no banco central MySQL
        public async Task<string> RealizarLoginRealAsync(string email, string senha)
        {
            try
            {
                // Monta os dados para enviar ao servidor (Java/PHP)
                var dadosLogin = new { email = email, senha = senha };
                string jsonEnvio = JsonSerializer.Serialize(dadosLogin);
                var conteudo = new StringContent(jsonEnvio, Encoding.UTF8, "application/json");

                // Faz a chamada na API
                var resposta = await _httpClient.PostAsync($"{BaseUrl}login.php", conteudo);

                if (resposta.IsSuccessStatusCode)
                {
                    // Lê a resposta do servidor (ex: "Passageiro", "Administrador" ou "Erro")
                    string perfilRetornado = await resposta.Content.ReadAsStringAsync();
                    return perfilRetornado;
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