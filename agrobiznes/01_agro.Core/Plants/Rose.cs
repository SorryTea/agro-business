using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    public class Rose : Plant, IPositioned
    {

        public Rose() : base("Róża", PlantType.Flower)
        {
            Price = 100;
            SalePrice = 123;
            MoistureLevel = 50;
            SunlightLevel = 50;
        }
        protected override void DoSpecificGrowth()
        {
            GrowthLevel += 10;
        }
    }
}
