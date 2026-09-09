using SQLite;

namespace MonitorLotacaoApp.Models
{
    public class RelatoModel
    {
        [PrimaryKey, AutoIncrement]
        public int IdRelato { get; set; }
        public string CodigoLinha { get; set; }
        public string NivelLotacao { get; set; }
        public DateTime Horario { get; set; }
        public string Status { get; set; } // Ex: "Pendente" ou "Sincronizado"
        public bool Sincronizado { get; set; }
    }
}