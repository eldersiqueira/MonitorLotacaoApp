using SQLite;
using System;

namespace MonitorLotacaoApp.Models
{
    public class RelatoModel
    {
        [PrimaryKey, AutoIncrement]
        public int IdRelato { get; set; }

        public string CodigoLinha { get; set; }
        public string NivelLotacao { get; set; }
        public DateTime Horario { get; set; }

        // Propriedades geoespaciais necessárias para a US 1.2
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string Status { get; set; } // Ex: "Pendente" ou "Sincronizado"
        public bool Sincronizado { get; set; }
    }
}