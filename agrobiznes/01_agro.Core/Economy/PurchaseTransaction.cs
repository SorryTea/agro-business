using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Transakcja typu zakup (koszt).
    /// Zmniejsza saldo konta.
    /// </summary>

    public sealed class PurchaseTransaction : Transaction
    {
        public override TransactionType Type => TransactionType.Purchase;

        public PurchaseTransaction(Money amount, TransactionCategory category, string description, DateTimeOffset? occurredAt = null)
            : base(amount, category, description, occurredAt)
        {
        }

        public override void Apply(Account account)
        {
            account.Debit(Amount);
        }

        public PurchaseTransaction() { }
    }
}
