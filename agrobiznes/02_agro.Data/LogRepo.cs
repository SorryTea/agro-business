using System;
using System.Collections.Generic;
using System.Linq;

namespace _02_agro.Data
{
    /// <summary>
    /// Klasa ta zapisuje powiadomienia/aktualizacje do pliku
    /// </summary>
    public class LogRepo
    {
        private readonly List<string> _logs = new();

        public void AddLog(string message)
        {
            // 1. Otwieramy połączenie (używając using, żeby samo się zamknęło)
            using (var db = new FarmContext())
            {
                // 2. Tworzymy nowy wpis
                var log = new SystemLog
                {
                    Message = message,
                    Date = DateTime.Now
                };

                db.SystemLogs.Add(log);

                db.SaveChanges();

                Console.WriteLine("Zapisano log do bazy!");
            }
        }

        public List<string> GetLogs(int takeLast = 200)
        {
            using (var db = new FarmContext())
            {
                // 1) Najpierw pobieramy surowe dane z bazy (bez formatowania w Select!)
                var rows = db.SystemLogs
                    .OrderByDescending(x => x.Date)
                    .Take(takeLast)
                    .Select(x => new { x.Date, x.Message })
                    .ToList(); // <-- tu wykonuje się SQL, dalej pracujemy w pamięci

                // 2) Odwracamy, żeby było od najstarszych do najnowszych
                rows.Reverse();

                // 3) Formatowanie robimy już po ToList() (czyli poza EF) -> bez błędu
                return rows
                    .Select(x => $"[{x.Date:HH:mm:ss}] {x.Message}")
                    .ToList();
            }
        }
    }
}
