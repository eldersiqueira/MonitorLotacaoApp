using System;
using System.Text.RegularExpressions;
using Microsoft.Maui.Controls;
using MonitorLotacaoApp.Services;

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
        string email = TxtEmail.Text?.Trim();
        string senha = TxtSenha.Text;

        // 1. Validação de campos vazios
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            await DisplayAlert("Atenção", "Por favor, preencha o e-mail e a senha para entrar.", "OK");
            return;
        }

        // 2. Validação do formato do e-mail
        if (!ValidarFormatoEmail(email))
        {
            await DisplayAlert("Formato Inválido", "O e-mail digitado não possui um formato válido.", "OK");
            return;
        }

        try
        {
            // 3. Autenticação real via API PHP / MySQL (ApiService)
            var resultado = await _apiService.RealizarLoginRealAsync(email, senha);

            if (resultado != null && resultado.sucesso)
            {
                // Armazena o ID e o nome do passageiro na sessão local para uso posterior nos relatos
                Preferences.Set("idPassageiro", resultado.idPassageiro);
                Preferences.Set("nomeUsuario", resultado.nome);

                await DisplayAlert("Sucesso", $"Bem-vindo de volta, {resultado.nome}!", "OK");

                // Limpa os campos
                TxtEmail.Text = string.Empty;
                TxtSenha.Text = string.Empty;

                // Direciona para a tela principal de seleção de linhas de ônibus
                await Navigation.PushAsync(new SelecionarLinhaPage());
            }
            else
            {
                string mensagemErro = resultado?.mensagem ?? "E-mail não cadastrado ou senha incorreta.";
                await DisplayAlert("Erro de Autenticação", mensagemErro, "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro de Comunicação", $"Não foi possível conectar ao servidor: {ex.Message}", "OK");
        }
    }

    // Método exigido pelo botão do XAML para ir para a tela de cadastro
    private async void OnIrParaCadastroClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new CadastroPage());
    }

    // Método exigido pelo botão do XAML para redefinição de senha
    private async void OnRedefinirSenhaClicked(object sender, EventArgs e)
    {
        string emailRecuperacao = await DisplayPromptAsync("Recuperação de Senha", "Digite o seu e-mail cadastrado para redefinir a senha:", "Enviar", "Cancelar", keyboard: Keyboard.Email);

        if (string.IsNullOrWhiteSpace(emailRecuperacao))
            return;

        if (!ValidarFormatoEmail(emailRecuperacao))
        {
            await DisplayAlert("Erro", "O e-mail informado não é válido.", "OK");
            return;
        }

        string novaSenha = await DisplayPromptAsync("Nova Senha", "Digite a sua nova senha (mínimo 8 caracteres, com maiúscula, minúscula, número e caractere especial):", "Salvar", "Cancelar");

        if (string.IsNullOrWhiteSpace(novaSenha))
            return;

        try
        {
            bool sucesso = await _apiService.RedefinirSenhaAsync(emailRecuperacao, novaSenha);

            if (sucesso)
            {
                await DisplayAlert("Sucesso", "Senha redefinida com sucesso! Faça login com a nova senha.", "OK");
            }
            else
            {
                await DisplayAlert("Aviso", "Não foi possível redefinir a senha. Verifique se o e-mail está correto.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Falha na comunicação com o servidor: {ex.Message}", "OK");
        }
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
}