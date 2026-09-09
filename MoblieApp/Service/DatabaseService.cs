using SQLite;
using MonitorLotacaoApp.Models;

namespace MonitorLotacaoApp.Services
{
    public class DatabaseService
    {
        private static SQLiteAsyncConnection _database;

        private static async Task Init()
        {
            if (_database != null)
                return;

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "monitor_lotacao.db3");
            _database = new SQLiteAsyncConnection(dbPath);

            // Cria as tabelas da Sprint 1 e Sprint 3
            await _database.CreateTableAsync<Passageiro>();
            await _database.CreateTableAsync<RelatoModel>();
        }

        // --- Métodos para Passageiros (Sprint 1) ---
        public static async Task<int> SalvarPassageiroAsync(Passageiro passageiro)
        {
            await Init();
            return await _database.InsertAsync(passageiro);
        }

        public static async Task<Passageiro> ObterPassageiroPorEmailAsync(string email)
        {
            await Init();
            return await _database.Table<Passageiro>()
                                  .Where(p => p.Email == email)
                                  .FirstOrDefaultAsync();
        }

        // --- Métodos para Relatos Offline (Sprint 3) ---
        public static async Task<int> SalvarRelatoOfflineAsync(RelatoModel relato)
        {
            await Init();
            relato.Horario = DateTime.Now;
            relato.Status = "Pendente";
            relato.Sincronizado = false;

            return await _database.InsertAsync(relato);
        }

        public static async Task<List<RelatoModel>> ObterRelatosPendentesAsync()
        {
            await Init();
            return await _database.Table<RelatoModel>()
                                  .Where(r => r.Sincronizado == false)
                                  .ToListAsync();
        }

        public static async Task AtualizarRelatoSincronizadoAsync(RelatoModel relato)
        {
            await Init();
            relato.Sincronizado = true;
            relato.Status = "Sincronizado";
            await _database.UpdateAsync(relato);
        }
    }
}