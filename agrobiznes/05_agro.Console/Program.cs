using System;
using _03_agro.Logic;

class Program
{
    static void Main(string[] args)
    {

        // Tworzymy i uruchamiamy silnik
        SimulationEngine engine = new SimulationEngine();

        Console.WriteLine("================================");
        Console.WriteLine("TEST FINANSÓW (Market + Finance)");
        Console.WriteLine("================================");

        Console.WriteLine($"Saldo start: {engine.State.Finance.Account.Balance}");

        // 1) Kup pomidory
        Console.WriteLine(engine.Market.KupPomidory(5));
        Console.WriteLine($"Saldo po kupnie pomidorów: {engine.State.Finance.Account.Balance}");

        // 2) Kup zraszacz
        Console.WriteLine(engine.Market.KupZraszacz());
        Console.WriteLine($"Saldo po kupnie zraszacza: {engine.State.Finance.Account.Balance}");

        // 3) Spróbuj kupić dużo (żeby zabrakło kasy)
        Console.WriteLine(engine.Market.KupPomidory(9999));
        Console.WriteLine($"Saldo po próbie kupna 9999 pomidorów: {engine.State.Finance.Account.Balance}");

        // 4) Sprzedaż wszystkiego
        Console.WriteLine(engine.Market.SprzedajWszystko());
        Console.WriteLine($"Saldo po sprzedaży: {engine.State.Finance.Account.Balance}");

        Console.WriteLine("================================");
        Console.WriteLine("Koniec testu. Wciśnij ENTER.");
        Console.ReadLine();

        engine.StartSimulation();

        // Blokujemy zamknięcie okna
        Console.ReadLine();

        // Sprzątamy przy wyjściu
        engine.StopSimulation();
    }
}
