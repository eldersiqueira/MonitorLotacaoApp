using System.Text.RegularExpressions;
using MonitorLotacaoApp.Models;
using MonitorLotacaoApp.Services;

namespace MonitorLotacaoApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
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

        var passageiroCadastrado = await DatabaseService.ObterPassageiroPorEmailAsync(email);

        if (passageiroCadastrado != null && passageiroCadastrado.Senha == senha)
        {
            await DisplayAlert("Sucesso", $"Bem-vindo de volta, {passageiroCadastrado.Nome}!", "OK");
        }
        else
        {
            await DisplayAlert("Erro de Autenticação", "E-mail não cadastrado ou senha incorreta.", "OK");
        }
    }

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
            await DisplayAlert("Formato Inválido", "Insira um endereço de e-mail válido para o cadastro.", "OK");
            return;
        }

        if (senha.Length < 6)
        {
            await DisplayAlert("Segurança da Senha", "A senha deve conter no mínimo 6 caracteres.", "OK");
            return;
        }

        var existente = await DatabaseService.ObterPassageiroPorEmailAsync(email);
        if (existente != null)
        {
            await DisplayAlert("Aviso", "Este e-mail já está cadastrado no sistema.", "OK");
            return;
        }

        Passageiro novoPassageiro = new Passageiro
        {
            Nome = nome,
            Email = email,
            Senha = senha
        };

        int resultado = await DatabaseService.SalvarPassageiroAsync(novoPassageiro);

        if (resultado > 0)
        {
            await DisplayAlert("Sucesso", $"Cadastro de {nome} realizado com sucesso no SQLite!", "OK");
            TxtNome.Text = string.Empty;
            TxtEmail.Text = string.Empty;
            TxtSenha.Text = string.Empty;
        }
        else
        {
            await DisplayAlert("Erro", "Não foi possível salvar o cadastro no dispositivo.", "OK");
        }
    }

    private bool ValidarFormatoEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            string padraoRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, padraoRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
}