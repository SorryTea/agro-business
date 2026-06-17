using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Dane finansowe zagregowane dla określonego okresu czasu.
    /// Wykorzystywane do podatków i raportów.
    /// </summary>

    public class FinancialPeriod
    {
        public string Label { get; set; } = string.Empty;

        public DateTimeOffset From { get; set; }
        public DateTimeOffset To { get; set; }

        public Money Revenue { get; set; }
        public Money Costs { get; set; }
        public Money Profit { get; set; }



    }
}
