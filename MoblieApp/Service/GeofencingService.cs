using Microsoft.Maui.Devices.Sensors;

namespace MonitorLotacaoApp.Services
{
    public static class GeofencingService
    {
        // Raio de tolerância em metros para considerar que o usuário está no trajeto (ex: 100 metros)
        private const double RaioToleranciaMetros = 100.0;

        // Exemplo de coordenadas de referência para uma das linhas do TCC (Ex: Terminal / Trajeto da Linha 01)
        private static readonly Location CoordenadaReferenciaExemplo = new Location(-23.550520, -46.633308);

        public static async Task<bool> ValidarPresencaNoTrajetoAsync()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                var localizacaoAtual = await Geolocation.Default.GetLocationAsync(request);

                if (localizacaoAtual == null)
                    return false;

                // Calcula a distância entre a posição atual do usuário e a rota monitorada
                double distanciaMetros = Location.CalculateDistance(
                    localizacaoAtual,
                    CoordenadaReferenciaExemplo,
                    DistanceUnits.Kilometers) * 1000;

                // Retorna verdadeiro se o usuário estiver dentro do raio estipulado de tolerância
                return distanciaMetros <= RaioToleranciaMetros;
            }
            catch (Exception)
            {
                // Em caso de falha no hardware de GPS, retorna falso por segurança
                return false;
            }
        }
    }
}