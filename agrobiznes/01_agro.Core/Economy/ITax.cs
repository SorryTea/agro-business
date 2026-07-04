using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    public interface ITax
    {
        Money CalculateTax(FinancialPeriod period);
    }
}
