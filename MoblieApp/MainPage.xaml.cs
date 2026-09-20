namespace MonitorLotacaoApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }

    private async void OnIrParaLoginAdminClicked(object sender, EventArgs e)
    {
        // Navega para a página de login dedicada à gestão (Sprint 5)
        await Navigation.PushAsync(new Views.LoginPage());
    }
}