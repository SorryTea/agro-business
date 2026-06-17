using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    /// <summary>
    /// Klasa błędu PozaZakresem, która ogranicza wartości podczas symulacji, aby nie były one skrajnie abstrakcyjne
    /// </summary>
    internal class ArgumentOutOfRangeException : Exception
    {
        public ArgumentOutOfRangeException(string message, string v) : base(message) { }
    }
}
