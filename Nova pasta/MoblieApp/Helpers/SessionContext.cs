namespace MonitorLotacao.Helpers
{
    public static class SessionContext
    {
        // Alterne esta flag para 'true' para testar direto como Administrador, 
        // ou 'false' para testar como Passageiro comum.
        public static bool IsAdminSimulation { get; set; } = false;

        // Retorna o perfil atual baseado na simulação
        public static string CurrentProfile => IsAdminSimulation ? "Administrador (Órgão de Transporte)" : "Passageiro";
    }
}