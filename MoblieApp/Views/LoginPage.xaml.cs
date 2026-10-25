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

    // Método acionado ao clicar no botão "Entrar" (Comunicação com o MySQL via PHP)
    private async void OnEntrarClicked(object sender, EventArgs e)
    {
        string email = TxtEmail.Text?.Trim();
        string senha = TxtSenha.Text;

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
            // Consome a API que valida no MySQL centralizado
            LoginResponse resultado = await _apiService.RealizarLoginRealAsync(email, senha);

            if (resultado != null && resultado.sucesso)
            {
                await DisplayAlert("Sucesso", $"Bem-vindo de volta, {resultado.nome}!", "OK");

                int tipoUsuario = resultado.tipo_usuario ?? 0;

                // Redirecionamento baseado no perfil (0 = Passageiro, 1 = Administrador)
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

    // Método acionado ao clicar no botão "Cadastrar Nova Conta"
    private async void OnCadastrarClicked(object sender, EventArgs e)
    {
        string nome = TxtNome.Text?.Trim();
        string email = TxtEmail.Text?.Trim();
        string senha = TxtSenha.Text;

        if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            await DisplayAlert("Atenção", "Preencha todos os campos (Nome, E-mail e Senha) para realizar o cadastro.", "OK");
            return;
        }

        if (!ValidarFormatoEmail(email))
        {
            await DisplayAlert("Formato Inválido", "Digite um endereço de e-mail válido para o cadastro.", "OK");
            return;
        }

        if (senha.Length < 6)
        {
            await DisplayAlert("Segurança da Senha", "A senha deve conter no mínimo 6 caracteres.", "OK");
            return;
        }

        try
        {
            bool cadastrado = await _apiService.CadastrarPassageiroAsync(nome, email, senha);

            if (cadastrado)
            {
                await DisplayAlert("Sucesso", "Registo efetuado com sucesso no MySQL!", "OK");
                TxtNome.Text = string.Empty;
                TxtEmail.Text = string.Empty;
                TxtSenha.Text = string.Empty;
            }
            else
            {
                await DisplayAlert("Erro", "Não foi possível efetuar o registo. O e-mail pode já estar em uso.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Exceção ao cadastrar: {ex.Message}", "OK");
        }
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