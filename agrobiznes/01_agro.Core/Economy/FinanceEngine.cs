using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Core of the economy module: manages transactions and the account balance.
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
        /// Applies a transaction: updates the balance and records it in history.
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
        /// Generates a financial report for the given period.
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

            // If there are no transactions, default to PLN.
            var currency = scope.FirstOrDefault()?.Amount.Currency ?? "PLN";

            decimal revenue = scope
                .Where(t => t.Type == TransactionType.Sale)
                .Sum(t => t.Amount.Amount);

            decimal costs = scope
                .Where(t => t.Type != TransactionType.Sale)
                .Sum(t => t.Amount.Amount);

            var revenueMoney = new Money(revenue, currency);
            var costsMoney = new Money(costs, currency);

            // Profit is max(0, revenue - costs) — kept simple for tax and reporting.
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
