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

    // Método acionado ao clicar no botão "Entrar" (Figura 3 do TCC)[cite: 38]
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
            // Consulta assíncrona no banco de dados local SQLite
            var passageiroCadastrado = await DatabaseService.ObterPassageiroPorEmailAsync(email);

            if (passageiroCadastrado != null && passageiroCadastrado.Senha == senha)
            {
                await DisplayAlert("Sucesso", $"Bem-vindo de volta, {passageiroCadastrado.Nome}!", "OK");

                // Navegação segura na thread principal para a tela de seleção de linhas (Sprint 2)[cite: 13, 39]
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Navigation.PushAsync(new SelecionarLinhaPage());
                });
            }
            else
            {
                await DisplayAlert("Erro de Autenticação", "E-mail não cadastrado ou senha incorreta.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Ocorreu um erro ao acessar o banco de dados: {ex.Message}", "OK");
        }
    }

    // Método acionado ao clicar no botão "Cadastrar Nova Conta" (Figura 4 e 5 do TCC)[cite: 39, 40]
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
            var existente = await DatabaseService.ObterPassageiroPorEmailAsync(email);
            if (existente != null)
            {
                await DisplayAlert("Erro", "Este e-mail já está cadastrado no sistema.", "OK");
                return;
            }

            var novoPassageiro = new Passageiro
            {
                Nome = nome,
                Email = email,
                Senha = senha
            };

            var resultado = await DatabaseService.SalvarPassageiroAsync(novoPassageiro);

            if (resultado > 0)
            {
                await DisplayAlert("Sucesso", "Cadastro de passageiro realizado com sucesso no SQLite!", "OK");
                TxtNome.Text = string.Empty;
                TxtEmail.Text = string.Empty;
                TxtSenha.Text = string.Empty;
            }
            else
            {
                await DisplayAlert("Erro", "Não foi possível salvar o cadastro no dispositivo.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Exceção ao cadastrar: {ex.Message}", "OK");
        }
    }

    // Função auxiliar para validação de Regex do E-mail (Figura 6 do TCC)[cite: 41]
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