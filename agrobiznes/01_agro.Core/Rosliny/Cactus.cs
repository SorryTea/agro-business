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
    public class Cactus : Rosliny, IPositioned
    {

        public Cactus() : base("Kaktus", TypRosliny.Sukulent)
        {
            Cena = 20;
            CenaSprzedazy = 40;
            PoziomNawodnienia = 50;
            PoziomNaslonecznienia = 50;
        }
        protected override void DoSpecificGrowth()
        {
            PoziomWzrostu += 5;
        }
    }
}
