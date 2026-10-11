using MonitorLotacaoApp.Services; // Para reconhecer o seu ApiService
using System.Text.Json; // Para decifrar a resposta do PHP

namespace MonitorLotacaoApp.Views;

public partial class LoginPage : ContentPage
{
    // Instanciamos o serviço que comunica com o PHP
    private readonly ApiService _apiService = new ApiService();

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            // Lê os valores digitados nos campos de texto da interface, conforme a sua nota
            string emailDigitado = EmailEntry.Text;
            string senhaDigitada = SenhaEntry.Text;

            if (string.IsNullOrWhiteSpace(emailDigitado) || string.IsNullOrWhiteSpace(senhaDigitada))
            {
                await DisplayAlert("Aviso", "Por favor, preencha o e-mail e a palavra-passe.", "OK");
                return;
            }

            // Chama o método real que envia os dados para o XAMPP
            string respostaDoServidor = await _apiService.RealizarLoginRealAsync(emailDigitado, senhaDigitada);

            if (respostaDoServidor != "ErroDeConexao" && respostaDoServidor != "Falha")
            {
                // Converte a string JSON devolvida pelo PHP num objeto analisável
                var dadosRetorno = JsonDocument.Parse(respostaDoServidor).RootElement;
                bool sucesso = dadosRetorno.GetProperty("sucesso").GetBoolean();

                if (sucesso)
                {
                    int tipoUsuario = dadosRetorno.GetProperty("tipo_usuario").GetInt32();

                    // O direcionamento com base no tipo de utilizador passa a ser real
                    if (tipoUsuario == 0) // Passageiro
                    {
                        await Shell.Current.GoToAsync("//PassageiroHomePage");
                    }
                    else if (tipoUsuario == 1) // Administrador
                    {
                        await Shell.Current.GoToAsync("//AdminDashboardPage");
                    }
                }
                else
                {
                    // Lê a mensagem que configurou no PHP ("Credenciais inválidas.")
                    string mensagemErro = dadosRetorno.GetProperty("mensagem").GetString();
                    await DisplayAlert("Acesso Negado", mensagemErro, "OK");
                }
            }
            else
            {
                await DisplayAlert("Erro", "Não foi possível conectar ao servidor. Verifique o endereço IP e o XAMPP.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Falha Estrutural", $"Erro no processamento: {ex.Message}", "OK");
        }
    }

    private async void OnCadastrarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CadastroPage));
    }
}