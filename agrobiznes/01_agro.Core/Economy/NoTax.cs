using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Brak podatku.
    /// </summary>

    public class NoTax : ITax
    {
        public Money CalculateTax(FinancialPeriod period)
        {
            return new Money(0m, period.Profit.Currency);
        }
    }
}
