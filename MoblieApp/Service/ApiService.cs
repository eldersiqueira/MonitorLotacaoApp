using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonitorLotacaoApp.Services
{
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
                var payload = new
                {
                    idPassageiro = idPassageiro,
                    nome = nome,
                    email = email,
                    tipo_usuario = tipoUsuario
                };

                var resposta = await _httpClient.PostAsJsonAsync($"{BaseUrl}atualizar_usuario.php", payload);

                if (resposta.IsSuccessStatusCode)
                {
                    string json = await resposta.Content.ReadAsStringAsync();
                    var resultado = JsonSerializer.Deserialize<ApiResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
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
                var payload = new
                {
                    idPassageiro = idPassageiro
                };

                var resposta = await _httpClient.PostAsJsonAsync($"{BaseUrl}deletar_usuario.php", payload);

                if (resposta.IsSuccessStatusCode)
                {
                    string json = await resposta.Content.ReadAsStringAsync();
                    var resultado = JsonSerializer.Deserialize<ApiResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    return resultado != null && resultado.sucesso;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao deletar utilizador: {ex.Message}");
            }
            return false;
        }
    }
}