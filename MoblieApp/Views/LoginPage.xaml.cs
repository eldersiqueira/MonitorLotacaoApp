namespace MonitorLotacaoApp.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        try
        {
            /* 
               NOTA PARA A INTEGRAÇÃO VISUAL:
               Quando o Emanuel finalizar a tela no XAML, comente as variáveis de teste abaixo 
               e utilize a leitura direta dos componentes visuais:
               string emailDigitado = EmailEntry.Text;
               string senhaDigitada = SenhaEntry.Text;
            */

            // Dados fixos configurados para a sua rodada de testes atual
            string emailTeste = "passageiro@monitor.com";
            string senhaTeste = "123456";

            // Simulação de validação de papéis que o Elivelton integrará com o MySQL no backend[cite: 15, 16]
            if (emailTeste == "passageiro@monitor.com" && senhaTeste == "123456")
            {
                // Rota absoluta definida no AppShell, direciona para a interface de toque único[cite: 19]
                await Shell.Current.GoToAsync("//PassageiroHomePage");
            }
            else if (emailTeste == "admin@monitor.com")
            {
                // Rota de acesso ao painel do órgão de transporte[cite: 22]
                await Shell.Current.GoToAsync("//AdminDashboardPage");
            }
            else
            {
                await DisplayAlert("Acesso Negado", "E-mail ou senha incorretos. Verifique suas credenciais.", "OK");
            }
        }
        catch (Exception ex)
        {
            // Previne o fechamento repentino do aplicativo caso a rota não seja encontrada no AppShell
            await DisplayAlert("Falha Estrutural", $"Erro no roteamento do sistema: {ex.Message}", "OK");
        }
    }

    private async void OnCadastrarClicked(object sender, EventArgs e)
    {
        // Navegação relativa para a página de Cadastro estipulada para a Sprint 1[cite: 23]
        await Shell.Current.GoToAsync(nameof(CadastroPage));
    }
}