using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonitorLotacaoApp.Services
{
    // 1. Classe DTO para espelhar o retorno JSON do script PHP de login
    public class LoginResponse
    {
        public bool sucesso { get; set; }
        public string mensagem { get; set; }
        public int? tipo_usuario { get; set; } // 0 = Passageiro, 1 = Administrador
        public string nome { get; set; }
    }

    public class ApiResponse
    {
        public bool sucesso { get; set; }
        public string mensagem { get; set; }
    }

    public class ApiService
    {
        private readonly HttpClient _httpClient;

        // Endereço base da API PHP no servidor MySQL local (XAMPP)
        // Use http://10.0.2.2/monitor-api/ para o Emulador Android ou http://localhost/monitor-api/ para Windows
        private const string BaseUrl = "http://10.0.2.2/monitor-api/";

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        // 2. Método principal de Autenticação
        public async Task<LoginResponse> RealizarLoginAsync(string email, string senha)
        {
            try
            {
                var parametros = new Dictionary<string, string>
                {
                    { "email", email },
                    { "senha", senha }
                };
                var conteudo = new FormUrlEncodedContent(parametros);

                var resposta = await _httpClient.PostAsync($"{BaseUrl}login.php", conteudo);

                if (resposta.IsSuccessStatusCode)
                {
                    string retornoJson = await resposta.Content.ReadAsStringAsync();
                    var resultado = JsonSerializer.Deserialize<LoginResponse>(retornoJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return resultado ?? new LoginResponse { sucesso = false, mensagem = "Erro ao desserializar JSON." };
                }
                return new LoginResponse { sucesso = false, mensagem = "Erro de conexão com o servidor MySQL." };
            }
            catch (Exception ex)
            {
                return new LoginResponse { sucesso = false, mensagem = $"Falha: {ex.Message}" };
            }
        }

        // 3. Alias para atender chamadas vindas de ViewModels (Evita erro CS1061)
        public async Task<LoginResponse> RealizarLoginRealAsync(string email, string senha)
        {
            return await RealizarLoginAsync(email, senha);
        }

        // 4. Método de Cadastro de Passageiro no MySQL
        public async Task<bool> CadastrarPassageiroAsync(string nome, string email, string senha)
        {
            try
            {
                var parametros = new Dictionary<string, string>
                {
                    { "nome", nome },
                    { "email", email },
                    { "senha", senha }
                };
                var conteudo = new FormUrlEncodedContent(parametros);

                var resposta = await _httpClient.PostAsync($"{BaseUrl}cadastrar.php", conteudo);

                if (resposta.IsSuccessStatusCode)
                {
                    string retornoJson = await resposta.Content.ReadAsStringAsync();
                    var resultado = JsonSerializer.Deserialize<ApiResponse>(retornoJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return resultado != null && resultado.sucesso;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no cadastro: {ex.Message}");
            }
            return false;
        }

        // 5. Método para Enviar Relato de Lotação (Sincronização com o MySQL)
        public async Task<bool> EnviarRelatoAsync(string codigoLinha, string nivelLotacao)
        {
            try
            {
                var payload = new
                {
                    CodigoLinha = codigoLinha,
                    NivelLotacao = nivelLotacao,
                    Horario = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Status = "Ativo"
                };

                var resposta = await _httpClient.PostAsJsonAsync($"{BaseUrl}sincronizar.php", payload);

                if (resposta.IsSuccessStatusCode)
                {
                    string retornoJson = await resposta.Content.ReadAsStringAsync();
                    var resultado = JsonSerializer.Deserialize<ApiResponse>(retornoJson, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return resultado != null && resultado.sucesso;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao sincronizar relato: {ex.Message}");
            }
            return false;
        }
    }
}