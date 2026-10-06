using Microsoft.Maui.Controls;
using MonitorLotacao.Helpers;
using System;

namespace MonitorLotacaoApp.Views
{
    public partial class AdminDashboardPage : ContentPage
    {
        public AdminDashboardPage()
        {
            InitializeComponent();
        }

        private async void OnSwitchToPassageiroClicked(object sender, EventArgs e)
        {
            // Inverte o estado de simulação na classe global de sessão
            SessionContext.IsAdminSimulation = false;

            // Reinicia a navegação para voltar à vista do passageiro
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}