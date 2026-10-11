using Microsoft.Maui.Devices.Sensors;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace MonitorLotacaoApp.Services
{
    public static class GeofencingService
    {
        // Raio de tolerância em metros. 100 metros é um ótimo padrão para áreas urbanas.
        private const double RaioToleranciaMetros = 100.0;

        /// <summary>
        /// Valida se a posição atual do smartphone está dentro do raio de tolerância de qualquer ponto do trajeto.
        /// </summary>
        /// <param name="trajetoLinha">Lista de coordenadas que formam a rota do ônibus.</param>
        public static async Task<bool> ValidarPresencaNoTrajetoAsync(List<Location> trajetoLinha)
        {
            try
            {
                // Requisita a localização com alta precisão
                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                var localizacaoAtual = await Geolocation.Default.GetLocationAsync(request);

                if (localizacaoAtual == null || trajetoLinha == null || !trajetoLinha.Any())
                    return false;

                // Verifica a distância do usuário para cada ponto da rota
                foreach (var pontoRota in trajetoLinha)
                {
                    double distanciaMetros = Location.CalculateDistance(
                        localizacaoAtual,
                        pontoRota,
                        DistanceUnits.Kilometers) * 1000;

                    // Se ele estiver a menos de 100m de QUALQUER ponto da rota, a localização é válida
                    if (distanciaMetros <= RaioToleranciaMetros)
                    {
                        return true;
                    }
                }

                // Se varreu toda a rota e não achou nenhum ponto próximo, ele está fora do trajeto
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro no GPS: {ex.Message}");
                // Em caso de falha no hardware, retorna falso para evitar envios falsos
                return false;
            }
        }
    }
}