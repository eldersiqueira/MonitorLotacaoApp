namespace MonitorLotacaoApp.Models
{
    public class RelatorioModel
    {
        public string CodigoLinha { get; set; }
        public string FaixaHoraria { get; set; }
        public string NivelLotacao { get; set; }
        public int TotalOcorrencias { get; set; }
    }

    public class RelatorioResponseDto
    {
        public bool Sucesso { get; set; }
        public string Periodo { get; set; }
        public List<RelatorioModel> DadosEstatisticos { get; set; }
    }
}