using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _01_agro.Core.Economy;

namespace _01_agro.Core
{
    /// <summary>
    /// Klasa Sprinkler dziedzicząca po klasie Device odpowiada za nawodnienie roślinek
    /// </summary>
    public class Sprinkler: Device
    {
        public Sprinkler()
        {
            Name = "Zraszacz ogrodowy";
            Cena = 1000;
        }
        public override void Tick(FarmState state)
        {
            // Jeśli maszyna jest włączona...
            if (IsOn)
            {
                // 1. Zwiększ wilgotność gleby (Globalnie)
                state.SoilMoisture += 5.0;

                // 2. Zabezpiecz, żeby nie utopić farmy (max 100%)
                if (state.SoilMoisture > 100) state.SoilMoisture = 100;

                // 3. Pobierz opłatę za wodę/prąd

                /*state.Finance.Apply(new PurchaseTransaction(
                    new Money(1m, "PLN"),
                    TransactionCategory.Water,
                    "Koszt wody - zraszacz"
                ));
                state.Finance.Apply(new PurchaseTransaction(
                        new Money(1m, "PLN"),
                        TransactionCategory.Energy,
                        "Koszt energii - zraszacz"
                    )
                );*/

            }
            
        }
    }
}

