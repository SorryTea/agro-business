using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Kategoria biznesowa transakcji (do raportów).
    /// </summary>

    public enum TransactionCategory
    {
        Energy,
        Water,
        Seeds,
        Sales,
        Fine,
        Other
    }

    /// <summary>
    /// Typ transakcji określający wpływ na saldo.
    /// </summary>

    public enum TransactionType
    {
        Purchase,
        Sale,
        Penalty
    }

    /// <summary>
    /// Abstrakcyjna klasa bazowa dla wszystkich transakcji finansowych.
    /// </summary>

    public abstract class Transaction
    {
        [Key]
        public Guid Id { get; private set; }
        public DateTimeOffset OccurredAt { get; private set; }
        public Money Amount { get; private set; }
        public string Description { get; }
        public TransactionCategory Category { get; private set; }
        public abstract TransactionType Type{ get; }

        protected Transaction(Money amount, TransactionCategory category, string description, DateTimeOffset? occurredAt = null)
        {
            Id = Guid.NewGuid();
            OccurredAt = occurredAt ?? DateTimeOffset.UtcNow;
            Amount = amount;
            Category = category;
            Description = description;
        }

        protected Transaction() { }

        public abstract void Apply(Account account);

        public override string ToString() => $"{OccurredAt:u} | {Type} | {Amount} | {Category} | {Description}";


    }

    /// <summary>
    /// Porównuje transakcje po dacie (najpierw najnowsze).
    /// </summary>
    public sealed class TransactionByDateDescComparer : IComparer<Transaction>
    {
        public int Compare(Transaction? x, Transaction? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return 1;
            if (y is null) return -1;

            return y.OccurredAt.CompareTo(x.OccurredAt); // DESC
        }
    }

    /// <summary>
    /// Porównuje transakcje po kwocie (największe najpierw).
    /// </summary>
    public sealed class TransactionByAmountDescComparer : IComparer<Transaction>
    {
        public int Compare(Transaction? x, Transaction? y)
        {
            if (ReferenceEquals(x, y)) return 0;
            if (x is null) return 1;
            if (y is null) return -1;

            // Jeśli waluty różne, to nie porównujemy "na pałę"
            if (!string.Equals(x.Amount.Currency, y.Amount.Currency, StringComparison.Ordinal))
                return string.Compare(x.Amount.Currency, y.Amount.Currency, StringComparison.Ordinal);

            return y.Amount.Amount.CompareTo(x.Amount.Amount); // DESC
        }
    }
}
