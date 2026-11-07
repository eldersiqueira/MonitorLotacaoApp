using System.Text.RegularExpressions;
using MonitorLotacaoApp.Services;

namespace MonitorLotacaoApp.Views;

public partial class CadastroPage : ContentPage
{
    private readonly ApiService _apiService;

    public CadastroPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
    }

    private async void OnCadastrarClicked(object sender, EventArgs e)
    {
        string nome = TxtNome.Text?.Trim();
        string email = TxtEmail.Text?.Trim();
        string senha = TxtSenha.Text;

        // 1. Validação de campos vazios
        if (string.IsNullOrWhiteSpace(nome) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            await DisplayAlert("Atenção", "Por favor, preencha todos os campos para realizar o cadastro.", "OK");
            return;
        }

        // 2. Validação do formato do e-mail
        if (!ValidarFormatoEmail(email))
        {
            await DisplayAlert("Formato Inválido", "Digite um endereço de e-mail válido para o cadastro.", "OK");
            return;
        }

        // 3. Validação rigorosa da política de senha forte
        string erroSenha = ValidarSenhaForte(senha);
        if (!string.IsNullOrEmpty(erroSenha))
        {
            await DisplayAlert("Segurança da Senha", erroSenha, "OK");
            return;
        }

        try
        {
            // 4. Registo no servidor MySQL via API PHP (ApiService)
            bool cadastradoComSucesso = await _apiService.CadastrarPassageiroAsync(nome, email, senha);

            if (cadastradoComSucesso)
            {
                await DisplayAlert("Sucesso", "Conta cadastrada com sucesso no MySQL!", "OK");

                TxtNome.Text = string.Empty;
                TxtEmail.Text = string.Empty;
                TxtSenha.Text = string.Empty;

                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Aviso do Servidor", "O servidor MySQL rejeitou o cadastro. O e-mail informado já pode estar em uso ou ocorreu uma falha na base central.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro de Comunicação", $"Não foi possível ligar ao servidor MySQL. Detalhe: {ex.Message}", "OK");
        }
    }

    private string ValidarSenhaForte(string senha)
    {
        if (string.IsNullOrWhiteSpace(senha) || senha.Length < 8)
            return "A segurança exige que a senha tenha no mínimo 8 caracteres.";

        if (!Regex.IsMatch(senha, @"[A-Z]"))
            return "A senha deve conter pelo menos uma letra maiúscula (A-Z).";

        if (!Regex.IsMatch(senha, @"[a-z]"))
            return "A senha deve conter pelo menos uma letra minúscula (a-z).";

        if (!Regex.IsMatch(senha, @"[0-9]"))
            return "A senha deve conter pelo menos um número (0-9).";

        if (!Regex.IsMatch(senha, @"[\W_]"))
            return "A senha deve conter pelo menos um caractere especial (ex: @, #, $, !, %).";

        return string.Empty;
    }

    private bool ValidarFormatoEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            string padrao = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, padrao, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    private async void OnVoltarLoginClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}