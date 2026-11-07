using System.Text.RegularExpressions;
using MonitorLotacaoApp.Models;
using MonitorLotacaoApp.Services;
using MonitorLotacaoApp.Views;

namespace MonitorLotacaoApp.Views;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _apiService;

    public LoginPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
    }

    private async void OnEntrarClicked(object sender, EventArgs e)
    {
        // Higienização profunda: remove espaços em branco antes, depois e quaisquer espaços internos acidentais
        string email = TxtEmail.Text?.Trim().Replace(" ", "") ?? string.Empty;
        string senha = TxtSenha.Text ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            await DisplayAlert("Atenção", "Por favor, preencha o e-mail e a senha para entrar.", "OK");
            return;
        }

        if (!ValidarFormatoEmail(email))
        {
            await DisplayAlert("Formato Inválido", "O e-mail digitado não possui um formato válido.", "OK");
            return;
        }

        try
        {
            // Validação com o serviço de API / Base de dados estruturada no TCC
            LoginResponse resultado = await _apiService.RealizarLoginRealAsync(email, senha);

            if (resultado != null && resultado.sucesso)
            {
                await DisplayAlert("Sucesso", $"Bem-vindo de volta, {resultado.nome}!", "OK");

                int tipoUsuario = resultado.tipo_usuario ?? 0;

                // Redirecionamento correto conforme o perfil: Administrador (1) ou Passageiro (0)
                if (tipoUsuario == 1)
                {
                    await Navigation.PushAsync(new AdminDashboardPage());
                }
                else
                {
                    await Navigation.PushAsync(new SelecionarLinhaPage());
                }
            }
            else
            {
                string msgErro = resultado?.mensagem ?? "Credenciais inválidas ou utilizador não cadastrado.";
                await DisplayAlert("Erro de Autenticação", msgErro, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro na conexão: {ex.Message}", "OK");
        }
    }

    private async void OnRedefinirSenhaClicked(object sender, EventArgs e)
    {
        string email = TxtEmail.Text?.Trim();

        if (string.IsNullOrWhiteSpace(email))
        {
            await DisplayAlert("Atenção", "Digite o seu e-mail no campo correspondente acima para iniciar a redefinição de senha.", "OK");
            return;
        }

        string novaSenha = await DisplayPromptAsync("Redefinição de Senha", "Digite a sua nova senha (mínimo 6 caracteres):", "Confirmar", "Cancelar", keyboard: Keyboard.Text);

        if (string.IsNullOrWhiteSpace(novaSenha) || novaSenha.Length < 6)
        {
            await DisplayAlert("Atenção", "A nova senha deve ter pelo menos 6 caracteres.", "OK");
            return;
        }

        bool sucesso = await _apiService.RedefinirSenhaAsync(email, novaSenha);

        if (sucesso)
        {
            await DisplayAlert("Sucesso", "Senha redefinida com sucesso!", "OK");
            TxtSenha.Text = string.Empty;
        }
        else
        {
            await DisplayAlert("Erro", "Não foi possível redefinir a senha. Verifique se o e-mail está cadastrado.", "OK");
        }
    }

    private async void OnIrParaCadastroClicked(object sender, EventArgs e)
    {
        // Navega para a tela dedicada de cadastro de passageiro
        await Navigation.PushAsync(new CadastroPage());
    }

    private bool ValidarFormatoEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            string padraoEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, padraoEmail, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
}