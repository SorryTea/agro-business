using System;
using _02_agro.Data;

namespace _02b_agro.Data.Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testowanie bazy danych...");

            // 1. Tworzymy instancję LogRepo
            var repo = new LogRepo();

            try
            {
                // 2. Próbujemy dodać log
                repo.AddLog("Testowy log");

                Console.WriteLine("SUKCES! Log został zapisany.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("BŁĄD: Nie udało się połączyć z bazą.");
                Console.WriteLine(ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);
                }
            }

            Console.ReadKey();
        }
    }
}
