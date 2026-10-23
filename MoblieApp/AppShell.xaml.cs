using Microsoft.Maui.Controls;
using MonitorLotacaoApp.Views;

namespace MonitorLotacaoApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Registo oficial das rotas do fluxo linear
            Routing.RegisterRoute("SelecaoLinhaPage", typeof(SelecaoLinhaPage));
            Routing.RegisterRoute("RelatoPage", typeof(RelatoPage));
            Routing.RegisterRoute("AdminDashboardPage", typeof(AdminDashboardPage));
        }
    }
}