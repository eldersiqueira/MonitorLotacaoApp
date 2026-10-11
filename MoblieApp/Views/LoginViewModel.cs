using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MonitorLotacaoApp.Services;
using System.Threading.Tasks;
using System;
using Microsoft.Maui.Controls;

namespace MonitorLotacaoApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly ApiService _apiService;

        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string senha;

        // Avisa a interface para atualizar o IsNotBusy sempre que o IsBusy mudar
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        private bool isBusy;

        public bool IsNotBusy => !IsBusy;

        // Avisa a interface para atualizar o TemErro sempre que a MensagemErro mudar
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TemErro))]
        private string mensagemErro;

        public bool TemErro => !string.IsNullOrEmpty(MensagemErro);

        public LoginViewModel()
        {
            _apiService = new ApiService();
        }

        [RelayCommand]
        public async Task RealizarLoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Senha))
            {
                MensagemErro = "Por favor, preencha o e-mail e a senha.";
                return;
            }

            IsBusy = true;
            MensagemErro = string.Empty;

            try
            {
                string respostaDoServidor = await _apiService.RealizarLoginRealAsync(Email, Senha);

                if (respostaDoServidor != "ErroDeConexao" && respostaDoServidor != "Falha")
                {
                    var dadosRetorno = System.Text.Json.JsonDocument.Parse(respostaDoServidor).RootElement;
                    bool sucesso = dadosRetorno.GetProperty("sucesso").GetBoolean();

                    if (sucesso)
                    {
                        int tipoUsuario = dadosRetorno.GetProperty("tipo_usuario").GetInt32();

                        // Usa o Dispatcher para evitar a quebra da árvore visual
                        Application.Current.Dispatcher.Dispatch(async () =>
                        {
                            if (tipoUsuario == 1)
                            {
                                // Navegação relativa para o Administrador (Sem as barras //)
                                await Shell.Current.GoToAsync("AdminDashboardPage");
                            }
                            else
                            {
                                // Navegação relativa para o Passageiro (Sem as barras //)
                                await Shell.Current.GoToAsync("SelecaoLinhaPage");
                            }
                        });

                        Email = string.Empty;
                        Senha = string.Empty;
                    }
                    else
                    {
                        MensagemErro = dadosRetorno.GetProperty("mensagem").GetString();
                    }
                }
                else
                {
                    MensagemErro = "Falha na comunicação com o servidor. Verifique sua conexão.";
                }
            }
            catch (System.Text.Json.JsonException)
            {
                MensagemErro = "Erro ao processar a resposta do servidor.";
            }
            catch (Exception ex)
            {
                MensagemErro = $"Erro inesperado: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        public async Task IrParaRedefinirSenhaAsync()
        {
            await Shell.Current.GoToAsync("RedefinirSenhaPage");
        }
    }
}