using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonitorLotacaoApp.Services 
{ 

    public class PontoItinerarioDto
{
    public int idPonto { get; set; }
    public string nomePonto { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
}

    public class LoginResponse
    {
        public bool sucesso { get; set; }
        public string mensagem { get; set; }
        public int? tipo_usuario { get; set; }
        public string nome { get; set; }
    }

    public class ApiResponse
    {
        public bool sucesso { get; set; }
        public string mensagem { get; set; }
    }

    public class UsuarioDto
    {
        public int idPassageiro { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public int tipo_usuario { get; set; }
    }

    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "http://10.0.2.2/monitor-api/";

        public ApiService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
        }

        public async Task<LoginResponse> RealizarLoginRealAsync(string email, string senha)
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
                    return resultado ?? new LoginResponse { sucesso = false, mensagem = "Erro ao processar resposta." };
                }
                return new LoginResponse { sucesso = false, mensagem = "Servidor retornou erro HTTP." };
            }
            catch (Exception ex)
            {
                return new LoginResponse { sucesso = false, mensagem = $"Falha de conexão: {ex.Message}" };
            }
        }

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

        public async Task<bool> RedefinirSenhaAsync(string email, string novaSenha)
        {
            try
            {
                var payload = new { email = email, nova_senha = novaSenha };
                var resposta = await _httpClient.PostAsJsonAsync($"{BaseUrl}redefinir_senha.php", payload);

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
                Console.WriteLine($"Erro ao redefinir senha: {ex.Message}");
            }
            return false;
        }

        public async Task<bool> EnviarRelatoAsync(string codigoLinha, string nivelLotacao)
        {
            try
            {
                var parametros = new Dictionary<string, string>
        {
            { "CodigoLinha", codigoLinha },
            { "NivelLotacao", nivelLotacao },
            { "Horario", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
            { "Status", "Ativo" }
        };
                var conteudo = new FormUrlEncodedContent(parametros);
                var resposta = await _httpClient.PostAsync($"{BaseUrl}sincronizar.php", conteudo);

                if (resposta.IsSuccessStatusCode)
                {
                    string retornoJson = await resposta.Content.ReadAsStringAsync();
                    var resultado = JsonSerializer.Deserialize<ApiResponse>(retornoJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return resultado != null && resultado.sucesso;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao sincronizar relato: {ex.Message}");
            }
            return false;
        }

        public async Task<List<PontoItinerarioDto>> ObterItinerarioAsync(string codigoLinha)
        {
            try
            {
                var resposta = await _httpClient.GetAsync($"{BaseUrl}listar_itinerario.php?codigoLinha={codigoLinha}");
                if (resposta.IsSuccessStatusCode)
                {
                    string json = await resposta.Content.ReadAsStringAsync();
                    var resultado = JsonSerializer.Deserialize<Dictionary<string, object>>(json);

                    if (resultado != null && resultado.ContainsKey("sucesso") && resultado["sucesso"].ToString() == "True")
                    {
                        var arrayPontos = ((JsonElement)resultado["itinerario"]).GetRawText();
                        return JsonSerializer.Deserialize<List<PontoItinerarioDto>>(arrayPontos, new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter itinerário: {ex.Message}");
            }
            return new List<PontoItinerarioDto>();
        }

        // --- GESTÃO DE ACESSOS (PERFIL ADM) ---

        public async Task<List<UsuarioDto>> ObterTodosUsuariosAsync()
        {
            try
            {
                var resposta = await _httpClient.GetAsync($"{BaseUrl}listar_usuarios.php");
                if (resposta.IsSuccessStatusCode)
                {
                    string json = await resposta.Content.ReadAsStringAsync();
                    var lista = JsonSerializer.Deserialize<List<UsuarioDto>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return lista ?? new List<UsuarioDto>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao listar utilizadores: {ex.Message}");
            }
            return new List<UsuarioDto>();
        }

        public async Task<bool> AtualizarUsuarioAsync(int idPassageiro, string nome, string email, int tipoUsuario)
        {
            try
            {
                var parametros = new Dictionary<string, string>
        {
            { "idPassageiro", idPassageiro.ToString() },
            { "nome", nome },
            { "email", email },
            { "tipo_usuario", tipoUsuario.ToString() }
        };
                var conteudo = new FormUrlEncodedContent(parametros);
                var resposta = await _httpClient.PostAsync($"{BaseUrl}atualizar_usuario.php", conteudo);

                if (resposta.IsSuccessStatusCode)
                {
                    string retornoJson = await resposta.Content.ReadAsStringAsync();
                    var resultado = JsonSerializer.Deserialize<ApiResponse>(retornoJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return resultado != null && resultado.sucesso;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao atualizar utilizador: {ex.Message}");
            }
            return false;
        }

        public async Task<bool> DeletarUsuarioAsync(int idPassageiro)
        {
            try
            {
                var parametros = new Dictionary<string, string>
        {
            { "idPassageiro", idPassageiro.ToString() }
        };
                var conteudo = new FormUrlEncodedContent(parametros);
                var resposta = await _httpClient.PostAsync($"{BaseUrl}deletar_usuario.php", conteudo);

                if (resposta.IsSuccessStatusCode)
                {
                    string retornoJson = await resposta.Content.ReadAsStringAsync();
                    var resultado = JsonSerializer.Deserialize<ApiResponse>(retornoJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return resultado != null && resultado.sucesso;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao deletar utilizador: {ex.Message}");
            }
            return false;
        }

        public class EstatisticasAdminResponse
        {
            public bool sucesso { get; set; }
            public int totalRelatos { get; set; }
            public List<HorarioCriticoDto> horariosCriticos { get; set; }
        }

        public class HorarioCriticoDto
        {
            public int hora { get; set; }
            public int quantidade { get; set; }
        }

        // Adicione este método dentro da classe ApiService
        public async Task<EstatisticasAdminResponse> ObterEstatisticasAsync()
        {
            try
            {
                var resposta = await _httpClient.GetAsync($"{BaseUrl}estatisticas_admin.php");
                if (resposta.IsSuccessStatusCode)
                {
                    string json = await resposta.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<EstatisticasAdminResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter estatísticas: {ex.Message}");
            }
            return null;
        }
    }
}