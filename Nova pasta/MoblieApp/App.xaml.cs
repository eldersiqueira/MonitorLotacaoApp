using MonitorLotacao.Helpers;
using MonitorLotacaoApp.Views;

namespace MonitorLotacaoApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Define o AppShell como a espinha dorsal visual e de navegação do app
            MainPage = new AppShell();

            // Retiramos a chamada CheckUserSessionAndNavigate() daqui, pois
            // o direcionamento inicial agora será responsabilidade da tela de Login.
        }
    }
}