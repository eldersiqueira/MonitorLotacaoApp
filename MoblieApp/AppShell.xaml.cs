using Microsoft.Maui.Controls;
using MonitorLotacaoApp.Views; // Certifique-se de ter suas telas criadas nesta pasta

namespace MonitorLotacaoApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registramos todas as telas do fluxo linear
            Routing.RegisterRoute("RedefinirSenhaPage", typeof(RedefinirSenhaPage));
            Routing.RegisterRoute("SelecaoLinhaPage", typeof(SelecionarLinhaPage));
            Routing.RegisterRoute("RelatoPage", typeof(PassageiroHomePage));

            // Se o Administrador ainda existir no projeto, mantemos a rota dele:
            Routing.RegisterRoute("AdminDashboardPage", typeof(AdminDashboardPage));
        }
    }
}