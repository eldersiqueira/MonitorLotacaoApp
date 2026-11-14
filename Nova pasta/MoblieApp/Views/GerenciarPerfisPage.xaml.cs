using MonitorLotacaoApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MonitorLotacaoApp.Views;

public partial class GerenciarPerfisPage : ContentPage
{
    private readonly ApiService _apiService;
    private List<UsuarioDto> _listaCompletaUsuarios;

    public GerenciarPerfisPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
        CarregarUsuarios();
    }

    private async void CarregarUsuarios()
    {
        // Busca todos os utilizadores cadastrados no MySQL centralizado
        _listaCompletaUsuarios = await _apiService.ObterTodosUsuariosAsync();

        if (_listaCompletaUsuarios != null)
        {
            CollectionUsuarios.ItemsSource = _listaCompletaUsuarios;
            LblFiltroAtivo.Text = $"A exibir: Todos os Utilizadores ({_listaCompletaUsuarios.Count})";
        }
        else
        {
            CollectionUsuarios.ItemsSource = new List<UsuarioDto>();
            LblFiltroAtivo.Text = "A exibir: Nenhum utilizador encontrado";
        }
    }

    private void OnFiltrarAdminTapped(object sender, EventArgs e)
    {
        if (_listaCompletaUsuarios == null) return;

        var admins = _listaCompletaUsuarios.Where(u => u.tipo_usuario == 1).ToList();
        CollectionUsuarios.ItemsSource = admins;
        LblFiltroAtivo.Text = $"A exibir: Perfil Administrador ({admins.Count})";
    }

    private void OnFiltrarPassageiroTapped(object sender, EventArgs e)
    {
        if (_listaCompletaUsuarios == null) return;

        var passageiros = _listaCompletaUsuarios.Where(u => u.tipo_usuario == 0).ToList();
        CollectionUsuarios.ItemsSource = passageiros;
        LblFiltroAtivo.Text = $"A exibir: Perfil Passageiro ({passageiros.Count})";
    }

    private async void OnEditarUsuarioClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is UsuarioDto usuario)
        {
            // Solicita novo nome via prompt interativo
            string novoNome = await DisplayPromptAsync("Editar Utilizador", "Atualize o nome do utilizador:", initialValue: usuario.nome);
            if (string.IsNullOrWhiteSpace(novoNome)) return;

            // Solicita novo tipo de usuário (0 para Passageiro, 1 para Admin)
            string novoTipoStr = await DisplayPromptAsync("Tipo de Perfil", "Digite 0 para Passageiro ou 1 para Administrador:", initialValue: usuario.tipo_usuario.ToString());
            if (!int.TryParse(novoTipoStr, out int novoTipo) || (novoTipo != 0 && novoTipo != 1))
            {
                await DisplayAlert("Erro", "Tipo de utilizador inválido. Use apenas 0 ou 1.", "OK");
                return;
            }

            // Chama o serviço de atualização na API MySQL
            bool sucesso = await _apiService.AtualizarUsuarioAsync(usuario.idPassageiro, novoNome, usuario.email, novoTipo);

            if (sucesso)
            {
                await DisplayAlert("Sucesso", "Utilizador atualizado com sucesso!", "OK");
                CarregarUsuarios(); // Recarrega a lista atualizada
            }
            else
            {
                await DisplayAlert("Erro", "Não foi possível atualizar o utilizador no servidor.", "OK");
            }
        }
    }

    private async void OnExcluirUsuarioClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is UsuarioDto usuario)
        {
            bool confirmar = await DisplayAlert("Confirmar Exclusão", $"Tem certeza de que deseja excluir o utilizador {usuario.nome}?", "Sim", "Não");
            if (confirmar)
            {
                // Chama o serviço de exclusão na API MySQL
                bool sucesso = await _apiService.DeletarUsuarioAsync(usuario.idPassageiro);

                if (sucesso)
                {
                    await DisplayAlert("Sucesso", "Utilizador removido com sucesso!", "OK");
                    CarregarUsuarios(); // Recarrega a lista atualizada
                }
                else
                {
                    await DisplayAlert("Erro", "Não foi possível excluir o utilizador no servidor.", "OK");
                }
            }
        }
    }
}