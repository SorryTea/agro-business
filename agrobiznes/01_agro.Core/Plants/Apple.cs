using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    public class Apple : Plant, IPositioned
    {

        public Apple() : base("Jabłoń", PlantType.Fruit)
        {
            Price = 10;
            SalePrice = 15;
            MoistureLevel = 50;
            SunlightLevel = 50;
        }

        protected override void DoSpecificGrowth()
        {
            GrowthLevel += 1;
        }
    }
}
