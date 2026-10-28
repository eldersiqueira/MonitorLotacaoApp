using MonitorLotacaoApp.Services;

namespace MonitorLotacaoApp.Views;

public partial class AdminGerenciarUsuariosPage : ContentPage
{
    private readonly ApiService _apiService;

    public AdminGerenciarUsuariosPage()
    {
        InitializeComponent();
        _apiService = new ApiService();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarUsuariosAsync();
    }

    private async void OnCarregarUsuariosClicked(object sender, EventArgs e)
    {
        await CarregarUsuariosAsync();
    }

    private async Task CarregarUsuariosAsync()
    {
        var usuarios = await _apiService.ObterTodosUsuariosAsync();
        CvUsuarios.ItemsSource = usuarios;
    }

    private async void OnEditarClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is UsuarioDto usuario)
        {
            if (usuario.idPassageiro <= 0)
            {
                await DisplayAlert("Erro", "ID do utilizador inválido.", "OK");
                return;
            }

            string novoNome = await DisplayPromptAsync("Editar Utilizador", "Alterar nome:", initialValue: usuario.nome);
            if (string.IsNullOrWhiteSpace(novoNome)) return;

            string novoEmail = await DisplayPromptAsync("Editar Utilizador", "Alterar e-mail:", initialValue: usuario.email);
            if (string.IsNullOrWhiteSpace(novoEmail)) return;

            string tipoStr = await DisplayPromptAsync("Editar Utilizador", "Tipo de Perfil (0 = Passageiro, 1 = ADM):", initialValue: usuario.tipo_usuario.ToString());
            if (!int.TryParse(tipoStr, out int novoTipo)) return;

            bool sucesso = await _apiService.AtualizarUsuarioAsync(usuario.idPassageiro, novoNome, novoEmail, novoTipo);
            if (sucesso)
            {
                await DisplayAlert("Sucesso", "Dados do utilizador atualizados no MySQL!", "OK");
                await CarregarUsuariosAsync();
            }
            else
            {
                await DisplayAlert("Erro", "Não foi possível atualizar o utilizador.", "OK");
            }
        }
    }

    private async void OnExcluirClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.CommandParameter is UsuarioDto usuario)
        {
            if (usuario.idPassageiro <= 0)
            {
                await DisplayAlert("Erro", "ID do utilizador inválido.", "OK");
                return;
            }

            bool confirmar = await DisplayAlert("Confirmação", $"Deseja realmente excluir o utilizador {usuario.nome}?", "Sim", "Não");
            if (confirmar)
            {
                bool sucesso = await _apiService.DeletarUsuarioAsync(usuario.idPassageiro);
                if (sucesso)
                {
                    await DisplayAlert("Sucesso", "Utilizador removido do banco MySQL!", "OK");
                    await CarregarUsuariosAsync();
                }
                else
                {
                    await DisplayAlert("Erro", "Não foi possível excluir o utilizador.", "OK");
                }
            }
        }
    }
}