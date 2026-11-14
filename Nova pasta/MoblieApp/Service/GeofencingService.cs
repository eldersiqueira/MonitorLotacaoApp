using Microsoft.Maui.Devices.Sensors;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MonitorLotacaoApp.Services
{
    public static class GeofencingService
    {
        // Tornamos o parâmetro opcional com um valor padrão caso não seja passado
        public static async Task<bool> ValidarPresencaNoTrajetoAsync(List<Location> trajetoLinha = null)
        {
            try
            {
                // Captura a localização atual do smartphone do passageiro (Premissa do projeto: GPS ativo)
                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                var localizacaoAtual = await Geolocation.Default.GetLocationAsync(request);

                if (localizacaoAtual == null)
                    return false;

                // Se nenhuma rota específica foi passada, usamos uma rota de teste padrão (ex: coordenadas de Mogi das Cruzes)
                if (trajetoLinha == null || trajetoLinha.Count == 0)
                {
                    trajetoLinha = new List<Location>
                    {
                        new Location(-23.5227, -46.1852), // Exemplo de ponto na região
                        new Location(-23.5300, -46.1900)
                    };
                }

                // Raio de tolerância em quilômetros (ex: 500 metros do trajeto oficial)
                double raioToleranciaKm = 0.5;

                foreach (var pontoRota in trajetoLinha)
                {
                    // Utiliza o cálculo nativo de distância do .NET MAUI (Fórmula de Haversine embutida)
                    double distanciaKm = Location.CalculateDistance(
                        localizacaoAtual.Latitude, localizacaoAtual.Longitude,
                        pontoRota.Latitude, pontoRota.Longitude,
                        DistanceUnits.Kilometers);

                    if (distanciaKm <= raioToleranciaKm)
                    {
                        return true; // O usuário está dentro do trajeto válido
                    }
                }

                return false; // Fora do raio permitido
            }
            catch
            {
                // Em caso de falha no sinal de GPS, retorna falso por segurança
                return false;
            }
        }
    }
}