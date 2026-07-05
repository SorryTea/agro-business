using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _01_agro.Core.Economy;

namespace _01_agro.Core
{
    public class Sprinkler : Device
    {
        public Sprinkler()
        {
            Name = "Zraszacz ogrodowy";
            Price = 1000;
        }
        public override void Tick(FarmState state)
        {
            if (IsOn)
            {
                state.SoilMoisture += 5.0;

                if (state.SoilMoisture > 100)
                {
                    state.SoilMoisture = 100;
                }

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

