using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonitorLotacaoApp.Views;

public partial class AdminUsuariosPage : ContentPage
{
    private readonly HttpClient _httpClient;
    public ObservableCollection<UsuarioModel> ListaUsuarios { get; set; }

    public AdminUsuariosPage()
    {
        InitializeComponent();
        _httpClient = new HttpClient();
        ListaUsuarios = new ObservableCollection<UsuarioModel>();

        // Define o contexto de dados para a CollectionView no XAML
        BindingContext = this;
    }

    // Executado automaticamente sempre que a página do painel de administração é exibida
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CarregarUsuariosAsync();
    }

    // Método que consome o endpoint PHP para listar os utilizadores registados no MySQL
    private async Task CarregarUsuariosAsync()
    {
        try
        {
            // URL do endpoint PHP centralizado (utilize '10.0.2.2' para o emulador Android)
            string url = "http://10.0.2.2/monitor-api/listar_usuarios.php";

            var resposta = await _httpClient.GetStringAsync(url);

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var usuariosDoServidor = JsonSerializer.Deserialize<List<UsuarioModel>>(resposta, options);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                ListaUsuarios.Clear();
                if (usuariosDoServidor != null)
                {
                    foreach (var usuario in usuariosDoServidor)
                    {
                        ListaUsuarios.Add(usuario);
                    }
                }
            });
        }
        catch (Exception ex)
        {
            await DisplayAlert("Erro de Conexão", $"Não foi possível carregar a lista de utilizadores: {ex.Message}", "OK");
        }
    }

    // Ação acionada quando o administrador clica no botão de editar um utilizador
    private async void OnEditarUsuarioClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is UsuarioModel usuarioSelecionado)
        {
            string novoNome = await DisplayPromptAsync("Editar Utilizador", "Atualize o nome:", initialValue: usuarioSelecionado.Nome);
            if (string.IsNullOrWhiteSpace(novoNome)) return;

            string novoEmail = await DisplayPromptAsync("Editar Utilizador", "Atualize o e-mail:", initialValue: usuarioSelecionado.Email);
            if (string.IsNullOrWhiteSpace(novoEmail)) return;

            string novoTipoStr = await DisplayPromptAsync("Editar Utilizador", "Tipo (0 = Passageiro, 1 = Admin):", initialValue: usuarioSelecionado.TipoUsuario.ToString());
            if (!int.TryParse(novoTipoStr, out int novoTipo)) return;

            await EnviarAtualizacaoServidorAsync(usuarioSelecionado.IdPassageiro, novoNome, novoEmail, novoTipo);
        }
    }

    // Envia a alteração dos dados do utilizador para a API PHP via POST
    private async Task EnviarAtualizacaoServidorAsync(int id, string nome, string email, int tipoUsuario)
    {
        try
        {
            var dados = new Dictionary<string, string>
            {
                { "idPassageiro", id.ToString() },
                { "nome", nome },
                { "email", email },
                { "tipo_usuario", tipoUsuario.ToString() }
            };

            var content = new FormUrlEncodedContent(dados);
            string url = "http://10.0.2.2/monitor-api/editar_usuario.php";

            var resposta = await _httpClient.PostAsync(url, content);

            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                var resultado = JsonSerializer.Deserialize<ResumoResponseDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (resultado != null && resultado.Sucesso)
                {
                    await DisplayAlert("Sucesso", resultado.Mensagem, "OK");
                    await CarregarUsuariosAsync(); // Recarrega a grelha de dados
                }
                else
                {
                    await DisplayAlert("Aviso", resultado?.Mensagem ?? "Erro ao atualizar.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Exceção", $"Erro na requisição: {ex.Message}", "OK");
        }
    }

    // Método acionado pelo botão "Remover" na interface XAML (Resolve o erro MAUIX2002)
    private async void OnRemoverUserClicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is UsuarioModel usuarioSelecionado)
        {
            bool confirmar = await DisplayAlert("Confirmar Exclusão", $"Deseja realmente remover o utilizador {usuarioSelecionado.Nome}?", "Sim", "Não");

            if (confirmar)
            {
                await ExcluirUsuarioServidorAsync(usuarioSelecionado.IdPassageiro);
            }
        }
    }

    // Envia a instrução de exclusão para o endpoint PHP no servidor
    private async Task ExcluirUsuarioServidorAsync(int idPassageiro)
    {
        try
        {
            var dados = new Dictionary<string, string>
            {
                { "idPassageiro", idPassageiro.ToString() }
            };

            var content = new FormUrlEncodedContent(dados);
            string url = "http://10.0.2.2/monitor-api/remover_usuario.php";

            var resposta = await _httpClient.PostAsync(url, content);

            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                var resultado = JsonSerializer.Deserialize<ResumoResponseDto>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (resultado != null && resultado.Sucesso)
                {
                    await DisplayAlert("Sucesso", resultado.Mensagem, "OK");
                    await CarregarUsuariosAsync(); // Atualiza a lista no ecrã
                }
                else
                {
                    await DisplayAlert("Aviso", resultado?.Mensagem ?? "Não foi possível remover o utilizador.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Exceção", $"Erro de ligação com o servidor: {ex.Message}", "OK");
        }
    }
}

// DTO e Modelos de dados mapeados para a base de dados MySQL
public class UsuarioModel
{
    [JsonPropertyName("idPassageiro")]
    public int IdPassageiro { get; set; }

    [JsonPropertyName("nome")]
    public string Nome { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }

    [JsonPropertyName("tipo_usuario")]
    public int TipoUsuario { get; set; } // 0 = Passageiro, 1 = Administrador
}

public class ResumoResponseDto
{
    [JsonPropertyName("sucesso")]
    public bool Sucesso { get; set; }

    [JsonPropertyName("mensagem")]
    public string Mensagem { get; set; }
}