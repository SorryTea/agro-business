using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Transakcja typu sprzedaż (przychód).
    /// Zwiększa saldo konta.
    /// </summary>

    public sealed class SaleTransaction : Transaction
    {
        public override TransactionType Type => TransactionType.Sale;

        public SaleTransaction(Money amount, TransactionCategory category, string description, DateTimeOffset? occurredAt = null)
            : base(amount, category, description, occurredAt)
        {
        }

        public SaleTransaction() { }

        public override void Apply(Account account)
        {
            account.Credit(Amount);
        }
    }
}
