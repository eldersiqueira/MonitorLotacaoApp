using SQLite;

namespace MonitorLotacaoApp.Models
{
    public class AdministradorModel
    {
        [PrimaryKey, AutoIncrement]
        public int IdAdministrador { get; set; }
        public string Nome { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
    }
}