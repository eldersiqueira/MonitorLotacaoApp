using System.Text.RegularExpressions;
using MonitorLotacaoApp.Models;
using MonitorLotacaoApp.Services;
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
        string email = TxtEmail.Text?.Trim();
        string senha = TxtSenha.Text;
        try
        {
            // Lê os valores digitados nos campos de texto da interface, conforme a sua nota
            string emailDigitado = EmailEntry.Text;
            string senhaDigitada = SenhaEntry.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
        {
            await DisplayAlert("Atenção", "Por favor, preencha o e-mail e a senha para entrar.", "OK");
            return;
        }
            if (string.IsNullOrWhiteSpace(emailDigitado) || string.IsNullOrWhiteSpace(senhaDigitada))
            {
                await DisplayAlert("Aviso", "Por favor, preencha o e-mail e a palavra-passe.", "OK");
                return;
            }

        if (!ValidarFormatoEmail(email))
        {
            await DisplayAlert("Formato Inválido", "O e-mail digitado não possui um formato válido.", "OK");
            return;
        }
            // Chama o método real que envia os dados para o XAMPP
            string respostaDoServidor = await _apiService.RealizarLoginRealAsync(emailDigitado, senhaDigitada);

        var passageiroCadastrado = await DatabaseService.ObterPassageiroPorEmailAsync(email);

        if (passageiroCadastrado != null && passageiroCadastrado.Senha == senha)
        {
            await DisplayAlert("Sucesso", $"Bem-vindo de volta, {passageiroCadastrado.Nome}!", "OK");

            // CORREÇÃO: Realiza a navegação para a tela de seleção de linhas da Sprint 2
            await Navigation.PushAsync(new SelecionarLinhaPage());
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
            if (respostaDoServidor != "ErroDeConexao" && respostaDoServidor != "Falha")
            {
                // Converte a string JSON devolvida pelo PHP num objeto analisável
                var dadosRetorno = JsonDocument.Parse(respostaDoServidor).RootElement;
                bool sucesso = dadosRetorno.GetProperty("sucesso").GetBoolean();

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
                if (sucesso)
                {
                    int tipoUsuario = dadosRetorno.GetProperty("tipo_usuario").GetInt32();

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
            string padraoRegex = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, padraoRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
            await DisplayAlert("Falha Estrutural", $"Erro no processamento: {ex.Message}", "OK");
        }
    }

    private async void OnCadastrarClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CadastroPage));
    }
}