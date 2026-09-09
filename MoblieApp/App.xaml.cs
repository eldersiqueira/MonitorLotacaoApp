namespace MonitorLotacaoApp;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Envolve a LoginPage em uma NavigationPage para permitir transições de tela
        MainPage = new NavigationPage(new Views.LoginPage());
    }
}