using System.Text;
using System.Text.Json;

namespace MonitorLotacaoApp.Views;

public partial class RedefinirSenhaPage : ContentPage
{
    public RedefinirSenhaPage()
    {
        InitializeComponent();
    }

    private async void OnRedefinirClicked(object sender, EventArgs e)
    {
        var dados = new { email = TxtEmail.Text, nova_senha = TxtNovaSenha.Text };
        var json = JsonSerializer.Serialize(dados);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            HttpClient client = new HttpClient();
            // Use 10.0.2.2 para emulador ou o IP real da sua máquina para telemóvel físico
            var resposta = await client.PostAsync("http://10.0.2.2/monitor-api/redefinir_senha.php", content);

            if (resposta.IsSuccessStatusCode)
            {
                await DisplayAlert("Sucesso", "Senha alterada com sucesso! Faça login novamente.", "OK");
                await Navigation.PopAsync();
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro", $"Não foi possível conectar ao servidor: {ex.Message}", "OK");
        }
    }
}