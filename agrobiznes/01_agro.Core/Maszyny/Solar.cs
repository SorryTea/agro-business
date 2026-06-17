using _01_agro.Core.Economy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    /// <summary>
    /// Klasa Solar jako klasa dziedzicząca po device, która będzie odpowiadała za nasłonecznienie roślinek
    /// </summary>
    public class Solar : Device
    {
        public Solar()
        {
            Name = "Lampa UV";
            Cena = 500;
        }
        public override void Tick(FarmState state)
        {
            // Jeśli maszyna jest włączona...
            if (IsOn)
            {
                // 1. Zwiększ poziom UV
                state.LightLevel += 5.0;

                // 2. Zabezpieczenie
                if (state.LightLevel > 100) state.LightLevel = 100;

                // 3. Pobierz opłatę za prąd (Tu jest problem bo placimy każdego ticka a powinniśmy raz na na miesiąc )

                /*state.Finance.Apply(new PurchaseTransaction(
                    new Money(1m, "PLN"),
                    TransactionCategory.Energy, 
                    "Koszt energii - lampa UV"
                    )
                );*/


            }

        }
    }
}
