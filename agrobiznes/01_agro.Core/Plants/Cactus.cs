using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    /// <summary>
    /// Roślina - kaktus z atrybutami
    /// </summary>
    public class Cactus : Plant, IPositioned
    {

        public Cactus() : base("Kaktus", PlantType.Succulent)
        {
            Price = 20;
            SalePrice = 40;
            MoistureLevel = 50;
            SunlightLevel = 50;
        }
        protected override void DoSpecificGrowth()
        {
            GrowthLevel += 5;
        }
    }
}
