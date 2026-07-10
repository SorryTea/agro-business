using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    public abstract class Device : ITickable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public bool IsOn { get; set; }

        public float Price { get; set; }
        public abstract void Tick(FarmState state);

    }
}
