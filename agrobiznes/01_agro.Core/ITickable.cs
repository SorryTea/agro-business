using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    /// <summary>
    /// Interfejs, który pozwala na odświeżanie obietku, a co za tym idzie symulację jego zmian w czasie
    /// </summary>
    public interface ITickable
    {
        // Metoda wywoływana przez silnik w każdej klatce symulacji
        void Tick(FarmState state);
    }
}
