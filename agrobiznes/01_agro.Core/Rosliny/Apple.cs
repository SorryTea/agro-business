using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    /// <summary>
    /// Roślina - Jabłko z atrybutami
    /// </summary>
    public class Apple : Rosliny, IPositioned
    {

        public Apple() : base("Jabłoń", TypRosliny.Owoc)
        {
            Cena = 10;
            CenaSprzedazy = 15;
            PoziomNawodnienia = 50;
            PoziomNaslonecznienia = 50;
        }

        protected override void DoSpecificGrowth()
        {
            PoziomWzrostu += 1;
        }
    }
}
