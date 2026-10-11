using Microsoft.Extensions.Logging;
using MonitorLotacaoApp.Services;
using MonitorLotacaoApp.ViewModels;
// using MonitorLotacaoApp.Views; // Descomente esta linha quando tiver a pasta Views criada

namespace MonitorLotacaoApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            // =========================================================
            // CONFIGURAÇÃO DE INJEÇÃO DE DEPENDÊNCIA (DI)
            // =========================================================

            // 1. Serviços (Services)
            // AddSingleton: Cria apenas uma instância do serviço para toda a vida útil da aplicação.
            // Ideal para o ApiService, pois queremos reaproveitar a mesma ligação HTTP (HttpClient) 
            // e poupar recursos do smartphone.
            builder.Services.AddSingleton<ApiService>();

            // 2. ViewModels
            // AddTransient: Cria uma nova instância sempre que for solicitada.
            // Ideal para ViewModels de ecrãs como o Login, garantindo que os dados (como a palavra-passe)
            // sejam limpos quando o utilizador sai e volta ao ecrã.
            builder.Services.AddTransient<LoginViewModel>();

            // 3. Views (Páginas)
            // Também devemos registar a página que vai receber a ViewModel.
            // builder.Services.AddTransient<LoginPage>(); // Iremos criar esta página a seguir

            builder.Services.AddTransient<RelatoViewModel>();

            return builder.Build();
        }
    }
}