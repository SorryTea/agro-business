using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    public interface IPositioned
    {
        int Row { get; set; }
        int Col { get; set; }
    }   
}
