using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    /// <summary>
    /// Something the simulation advances once per tick, letting it evolve over time.
    /// </summary>
    public interface ITickable
    {
        void Tick(FarmState state);
    }
}
