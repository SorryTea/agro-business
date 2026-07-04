using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _01_agro.Core;


namespace _01_agro.Core
{
    public class Tomato : Plant, IPositioned
    {


        public Tomato() : base("Pomidor", PlantType.Vegetable)
        {
            Price = 2;
            SalePrice = 2.5f;
            MoistureLevel = 50;
            SunlightLevel = 50;
        }

        protected override void DoSpecificGrowth()
        {
            GrowthLevel += 2;
        }
    }
}


