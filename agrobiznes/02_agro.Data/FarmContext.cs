using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using _01_agro.Core;
using _01_agro.Core.Economy;


namespace _02_agro.Data
{
    /// <summary>
    /// Klasa ta tworzy liste powiadomień/aktualizacji, roślin oraz urządzeń
    /// </summary>
    public class FarmContext : DbContext
    {

        // Tabela z logami
        public DbSet<SystemLog> SystemLogs { get; set; }
        // Rośliny i maszyny
        public DbSet<Rosliny> Plants { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
    }
}
