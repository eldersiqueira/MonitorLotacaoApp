using SQLite;
using MonitorLotacaoApp.Models;

namespace MonitorLotacaoApp.Services
{
    public static class DatabaseService
    {
        static SQLiteAsyncConnection _database;

        public static async Task Init()
        {
            if (_database != null)
                return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "lugus_lotacao.db3");
            _database = new SQLiteAsyncConnection(dbPath);

            // Criação das tabelas do sistema no SQLite local (.NET MAUI)
            await _database.CreateTableAsync<Passageiro>();
            await _database.CreateTableAsync<AdministradorModel>();
            await _database.CreateTableAsync<RelatoModel>();

            // Insere um administrador padrão caso a tabela esteja vazia (para testes)
            var totalAdmins = await _database.Table<AdministradorModel>().CountAsync();
            if (totalAdmins == 0)
            {
                await _database.InsertAsync(new AdministradorModel
                {
                    Nome = "Gestor de Transporte",
                    Login = "admin",
                    Senha = "123456"
                });
            }
        }

        // --- MÉTODOS DE PASSAGEIRO ---
        public static async Task<Passageiro> ObterPassageiroPorEmailAsync(string email)
        {
            await Init();
            return await _database.Table<Passageiro>().Where(p => p.Email == email).FirstOrDefaultAsync();
        }

        public static async Task<int> SalvarPassageiroAsync(Passageiro passageiro)
        {
            await Init();
            return await _database.InsertAsync(passageiro);
        }

        // --- MÉTODOS DE ADMINISTRADOR ---
        public static async Task<AdministradorModel> ObterAdministradorPorLoginAsync(string login)
        {
            await Init();
            return await _database.Table<AdministradorModel>().Where(a => a.Login == login).FirstOrDefaultAsync();
        }

        // --- MÉTODOS DE RELATOS OFFLINE ---
        public static async Task<int> SalvarRelatoAsync(RelatoModel relato)
        {
            await Init();
            return await _database.InsertAsync(relato);
        }

        public static async Task<List<RelatoModel>> ObterRelatosPendentesAsync()
        {
            await Init();
            return await _database.Table<RelatoModel>().Where(r => r.Sincronizado == false).ToListAsync();
        }

        public static async Task<int> AtualizarRelatoSincronizadoAsync(RelatoModel relato)
        {
            await Init();
            relato.Sincronizado = true;
            return await _database.UpdateAsync(relato);
        }
    }
}