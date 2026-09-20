namespace MonitorLotacaoApp.Models
{
    public class UsuarioModel
    {
        public int idPassageiro { get; set; }
        public string nome { get; set; }
        public string email { get; set; }
        public int tipo_usuario { get; set; }
    }

    public class UsuarioResponseDto
    {
        public bool sucesso { get; set; }
        public List<UsuarioModel> Usuarios { get; set; }
    }
}