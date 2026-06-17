using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Podatek liniowy.
    /// </summary>

    public class FlatTax : ITax
    {
        public decimal Rate { get; }

        public FlatTax(decimal rate)
        {
            if (rate < 0m || rate > 1m)
            {
                throw new System.ArgumentOutOfRangeException(nameof(rate));
            }

            Rate = rate;
        }

        public Money CalculateTax(FinancialPeriod period)
        {
            if (period.Profit.Amount <= 0m)
            {
                return new Money(0m, period.Profit.Currency);
            }

            return new Money(period.Profit.Amount * Rate, period.Profit.Currency);
        }
    }
}
