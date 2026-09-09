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

            await _database.CreateTableAsync<Passageiro>();
        }

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
    }
}