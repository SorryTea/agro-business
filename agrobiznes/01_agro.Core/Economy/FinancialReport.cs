using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Raport finansowy gotowy do wyświetlenia lub eksportu.
    /// Zawiera podsumowanie wyników.
    /// </summary>

    public class FinancialReport
    {
        public string Title { get; set; } = string.Empty;

        public Money Revenue { get; set; }
        public Money Costs { get; set; }
        public Money Profit { get; set; }
        public Money Tax { get; set; }
        public Money NetProfit { get; set; }
    }
}
