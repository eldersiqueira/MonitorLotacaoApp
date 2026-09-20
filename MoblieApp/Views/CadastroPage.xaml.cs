using System.Text;
using System.Text.Json;

namespace MonitorLotacaoApp.Views;

public partial class CadastroPage : ContentPage
{
    public CadastroPage()
    {
        InitializeComponent();
    }

    private async void OnSalvarCadastroClicked(object sender, EventArgs e)
    {
        string nome = TxtNome.Text?.Trim();
        string email = TxtEmail.Text?.Trim();
        string senha = TxtSenha.Text?.Trim();

        if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(senha))
        {
            await DisplayAlert("Aviso", "Por favor, preencha todos os campos para continuar.", "OK");
            return;
        }

        try
        {
            // Os nomes das chaves devem corresponder exatamente aos índices $_POST do PHP
            var dados = new Dictionary<string, string>
            {
                { "nome", nome },
                { "email", email },
                { "senha", senha }
            };

            var content = new FormUrlEncodedContent(dados);
            HttpClient client = new HttpClient();

            // Endereço correto para o emulador Android aceder ao XAMPP local
            string url = "http://10.0.2.2/monitor-api/cadastrar.php";

            var resposta = await client.PostAsync(url, content);

            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();

                System.Diagnostics.Debug.WriteLine($"JSON Resposta Cadastro: {json}");

                var resultado = JsonSerializer.Deserialize<CadastroResponseDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (resultado != null && resultado.Sucesso)
                {
                    await DisplayAlert("Sucesso", resultado.Mensagem, "OK");
                    await Navigation.PopAsync(); // Retorna para a tela de login após o registo bem-sucedido
                }
                else
                {
                    await DisplayAlert("Erro no Registo", resultado?.Mensagem ?? "Não foi possível efetuar o registo.", "OK");
                }
            }
            else
            {
                await DisplayAlert("Erro de Servidor", "O servidor PHP retornou um código de erro HTTP.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Exceção de Conexão", $"Erro ao comunicar com o servidor: {ex.Message}", "OK");
        }
    }
}

public class CadastroResponseDto
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; }
}