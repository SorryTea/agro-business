using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Transakcja typu kara lub koszt nadzwyczajny.
    /// Zmniejsza saldo konta.
    /// </summary>

    public sealed class PenaltyTransaction : Transaction
    {
        public override TransactionType Type => TransactionType.Penalty;

        public PenaltyTransaction(Money amount, TransactionCategory category, string description, DateTimeOffset? occurredAt = null)
            : base(amount, category, description, occurredAt)
        {
        }

        public override void Apply(Account account)
        {
            account.Debit(Amount);
        }

        public PenaltyTransaction() { }
    }
}
