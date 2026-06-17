using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Główny silnik modułu ekonomii.
    /// Zarządza transakcjami i saldem.
    /// </summary>

    public class FinanceEngine
    {
        private readonly List<Transaction> _transactions = new();

        public Account Account { get; }
        public ITax Tax { get; set; }

        public IReadOnlyList<Transaction> Transactions => _transactions;

        public FinanceEngine(Account account, ITax tax)
        {
            Account = account ?? throw new ArgumentNullException(nameof(account));
            Tax = tax ?? throw new ArgumentNullException(nameof(tax));
        }

        /// <summary>
        /// Stosuje transakcję: modyfikuje saldo i dodaje transakcję do historii.
        /// </summary>
        public void Apply(Transaction transaction)
        {
            if (transaction is null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            transaction.Apply(Account);
            _transactions.Add(transaction);
        }

        /// <summary>
        /// Generuje raport finansowy dla okresu.
        /// </summary>
        public FinancialReport GetReport(DateTimeOffset from, DateTimeOffset to, string? title = null)
        {
            if (to < from)
            {
                throw new ArgumentException("Koniec okresu nie może być wcześniejszy niż początek.");
            }

            var scope = _transactions
                .Where(t => t.OccurredAt >= from && t.OccurredAt <= to)
                .ToList();

            // Jeśli brak transakcji, przyjmujemy PLN.
            var currency = scope.FirstOrDefault()?.Amount.Currency ?? "PLN";

            decimal revenue = scope
                .Where(t => t.Type == TransactionType.Sale)
                .Sum(t => t.Amount.Amount);

            decimal costs = scope
                .Where(t => t.Type != TransactionType.Sale)
                .Sum(t => t.Amount.Amount);

            var revenueMoney = new Money(revenue, currency);
            var costsMoney = new Money(costs, currency);

            // Profit liczony jako max(0, revenue - costs) — prosto na potrzeby podatku i raportu.
            var profitValue = revenue - costs;
            var profitMoney = new Money(Math.Max(0m, profitValue), currency);

            var period = new FinancialPeriod
            {
                From = from,
                To = to,
                Revenue = revenueMoney,
                Costs = costsMoney,
                Profit = profitMoney
            };

            var taxMoney = Tax.CalculateTax(period);
            var netProfitMoney = new Money(Math.Max(0m, profitMoney.Amount - taxMoney.Amount), currency);

            return new FinancialReport
            {
                Title = title ?? "Raport finansowy",
                Revenue = revenueMoney,
                Costs = costsMoney,
                Profit = profitMoney,
                Tax = taxMoney,
                NetProfit = netProfitMoney
            };

        }
    }
}
