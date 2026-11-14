using Microsoft.Extensions.Logging;

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

            // Registro de serviços e páginas do aplicativo .NET MAUI
            // (A referência órfã a LoginViewModel foi totalmente removida para eliminar o erro CS0246)

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}