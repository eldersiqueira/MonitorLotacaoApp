using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonitorLotacaoApp.Views;

public partial class LoginPage : ContentPage
{
    private readonly HttpClient _httpClient;

    public LoginPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
    }

    // Método unificado de clique de login (Evita o conflito CS0111)
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string email = TxtEmail.Text?.Trim();
        string senha = TxtSenha.Text?.Trim();

        if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
        {
            await DisplayAlert("Aviso", "Por favor, preencha o e-mail e a palavra-passe.", "OK");
            return;
        }

        try
        {
            var dadosLogin = new Dictionary<string, string>
            {
                { "email", email },
                { "senha", senha }
            };

            var content = new FormUrlEncodedContent(dadosLogin);

            // URL do endpoint PHP (utilize '10.0.2.2' para o emulador Android ou o IP da rede local)
            string url = "http://10.0.2.2/monitor-api/login.php";

            var resposta = await _httpClient.PostAsync(url, content);

            if (resposta.IsSuccessStatusCode)
            {
                var jsonResponse = await resposta.Content.ReadAsStringAsync();

                // Registo na consola de depuração para validação da Sprint 5
                System.Diagnostics.Debug.WriteLine($"JSON Resposta Login: {jsonResponse}");

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var resultado = JsonSerializer.Deserialize<LoginResponseDto>(jsonResponse, options);

                if (resultado != null && resultado.Sucesso)
                {
                    await DisplayAlert("Sucesso", $"Bem-vindo de volta, {resultado.Nome}!", "OK");

                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        if (resultado.TipoUsuario == 1)
                        {
                            // Redireciona para o Painel Administrativo da Sprint 5
                            Application.Current.MainPage = new NavigationPage(new AdminUsuariosPage());
                        }
                        else
                        {
                            // Redireciona para o fluxo padrão do passageiro
                            Application.Current.MainPage = new NavigationPage(new RelatoriosPage());
                        }
                    });
                }
                else
                {
                    string mensagemErro = resultado?.Mensagem ?? "Credenciais inválidas.";
                    await DisplayAlert("Erro de Autenticação", mensagemErro, "OK");
                }
            }
            else
            {
                await DisplayAlert("Erro", "Não foi possível estabelecer ligação com o servidor PHP.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Exceção", $"Ocorreu um erro na requisição: {ex.Message}", "OK");
        }
    }

    // Método de navegação para a página de registo de novos utilizadores
    private async void OnCadastrarClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CadastroPage());
    }
}

// DTO de suporte com mapeamento explícito para o backend PHP
public class LoginResponseDto
{
    [JsonPropertyName("sucesso")]
    public bool Sucesso { get; set; }

    [JsonPropertyName("tipo_usuario")]
    public int TipoUsuario { get; set; } // 0 = Passageiro, 1 = Administrador[cite: 6]

    [JsonPropertyName("nome")]
    public string Nome { get; set; }

    [JsonPropertyName("mensagem")]
    public string Mensagem { get; set; }
}