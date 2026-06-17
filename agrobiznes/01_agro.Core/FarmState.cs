using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using _01_agro.Core.Economy;

namespace _01_agro.Core
{   //klasa FarmState czyli "stan świata": będzie przechowywała roślinki i maszyny, tick, pieniądze
    public class FarmState
    {
        // 1. Czas symulacji
        public long CurrentTick { get; set; } = 0;

        public double SoilMoisture { get; set; } = 20.0;//0-100 poziom nawodnienia gleby

        public double LightLevel { get; set; } = 20.0;

        
        // [NotMapped] oznacza, że nie chcemy tego zapisywać w tabeli FarmState w bazie.
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        // Tego nie zapisujemy do pliku, bo to kod, a nie dane!
        [JsonIgnore]
        public Action<string> Logger { get; set; }

      

        // 3. Główna lista obiektów symulacji - DODAWAJ OBIEKTY
        public List<Tomato> Tomatoes { get; set; } = new List<Tomato>();
        public List<Apple> Apples { get; set; } = new List<Apple>();
        public List<Cactus> Cactile { get; set; } = new List<Cactus>();
        public List<Rose> Roses { get; set; } = new List<Rose>();
        public List<Sprinkler> Sprinklers { get; set; } = new List<Sprinkler>();
        public List<Solar> Solars { get; set; } = new List<Solar>();
        public List<Sensor> Sensors { get; set; } = new List<Sensor>();

        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        [JsonIgnore]
        public FinanceEngine Finance { get; set; }


        public decimal BalanceAmount { get; set; } = 1000m;
        public string BalanceCurrency { get; set; } = "PLN";
        public virtual List<Transaction> Transactions { get; set; } = new List<Transaction>();

        // Konstruktor inicjalizujący listę (żeby uniknąć błędów null)
        public FarmState()
        {
            Finance = new FinanceEngine(
                new Account(new Money(BalanceAmount, BalanceCurrency)),
                new NoTax());
        }


    }
}
