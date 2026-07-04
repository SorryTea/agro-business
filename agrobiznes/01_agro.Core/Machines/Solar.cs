using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _01_agro.Core.Economy;

namespace _01_agro.Core
{
    public class Solar : Device
    {
        public Solar()
        {
            Name = "Lampa UV";
            Price = 500;
        }
        public override void Tick(FarmState state)
        {
            if (IsOn)
            {
                state.LightLevel += 5.0;

                if (state.LightLevel > 100)
                {
                    state.LightLevel = 100;
                }

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
