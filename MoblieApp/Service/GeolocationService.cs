using System;
using System.Threading.Tasks;
using Microsoft.Maui.Devices.Sensors;

namespace MonitorLotacaoApp.Services
{
    public class GeolocationService
    {
        // Método para capturar a localização atual do usuário
        public async Task<Location> ObterLocalizacaoAtualAsync()
        {
            try
            {
                // Verifica as permissões de GPS antes de tentar aceder
                var statusPermissao = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

                if (statusPermissao != PermissionStatus.Granted)
                {
                    statusPermissao = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                }

                if (statusPermissao == PermissionStatus.Granted)
                {
                    // Configura a precisão do GPS (High para garantir que está no ônibus)
                    var requisicao = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));

                    // Pega a coordenada nativa do aparelho
                    var localizacao = await Geolocation.Default.GetLocationAsync(requisicao);

                    return localizacao; // Retorna Latitude e Longitude
                }

                return null; // Permissão negada pelo usuário
            }
            catch (FeatureNotSupportedException)
            {
                // O smartphone não tem hardware de GPS
                Console.WriteLine("Hardware de GPS não suportado neste dispositivo.");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao obter localização: {ex.Message}");
                return null;
            }
        }
    }
}