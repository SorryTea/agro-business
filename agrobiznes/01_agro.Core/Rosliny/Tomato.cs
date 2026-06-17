using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _01_agro.Core;


namespace _01_agro.Core
{
    /// <summary>
    /// Roślina - Pomidor z atrybutami
    /// </summary>
    public class Tomato : Rosliny, IPositioned
    {
        

        public Tomato() : base("Pomidor", TypRosliny.Warzywo)
        {
            Cena = 2;
            CenaSprzedazy = 2.5f;
            PoziomNawodnienia = 50;
            PoziomNaslonecznienia = 50;
        }

        protected override void DoSpecificGrowth()
        {
            PoziomWzrostu += 2;
        }
    }
}


