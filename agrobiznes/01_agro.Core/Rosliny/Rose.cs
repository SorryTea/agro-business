using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    /// <summary>
    /// Roślina - róża z atrybutami
    /// </summary>
    public class Rose : Rosliny, IPositioned
    {

        public Rose() : base("Róża", TypRosliny.Kwiat)
        {
            Cena = 100;
            CenaSprzedazy = 123;
            PoziomNawodnienia = 50;
            PoziomNaslonecznienia = 50;
        }
        protected override void DoSpecificGrowth()
        {
            PoziomWzrostu += 10;
        }
    }
}
