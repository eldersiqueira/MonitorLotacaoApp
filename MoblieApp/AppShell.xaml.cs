namespace MonitorLotacaoApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Removemos as rotas de Passageiro, Admin e Login daqui,
            // pois elas já existem como <ShellContent> no AppShell.xaml.

            // Registramos APENAS páginas secundárias/modais:
            Routing.RegisterRoute(nameof(Views.CadastroPage), typeof(Views.CadastroPage));
        }
    }
}