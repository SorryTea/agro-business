using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Business category of a transaction (used for reports).
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
    /// Transaction kind that determines how the balance is affected.
    /// </summary>

    public enum TransactionType
    {
        Purchase,
        Sale,
        Penalty
    }

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
    [JsonDerivedType(typeof(PurchaseTransaction), "purchase")]
    [JsonDerivedType(typeof(SaleTransaction), "sale")]
    [JsonDerivedType(typeof(PenaltyTransaction), "penalty")]
    public abstract class Transaction
    {
        [Key]
        [JsonInclude]
        public Guid Id { get; private set; }
        [JsonInclude]
        public DateTimeOffset OccurredAt { get; private set; }
        [JsonInclude]
        public Money Amount { get; private set; } = new Money(0m, "PLN");
        [JsonInclude]
        public string Description { get; private set; } = string.Empty;
        [JsonInclude]
        public TransactionCategory Category { get; private set; }
        [JsonIgnore]
        public abstract TransactionType Type { get; }

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
    /// Orders transactions by date, newest first.
    /// </summary>
    public sealed class TransactionByDateDescComparer : IComparer<Transaction>
    {
        public int Compare(Transaction? x, Transaction? y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x is null)
            {
                return 1;
            }

            if (y is null)
            {
                return -1;
            }

            return y.OccurredAt.CompareTo(x.OccurredAt);
        }
    }

    /// <summary>
    /// Orders transactions by amount, largest first.
    /// </summary>
    public sealed class TransactionByAmountDescComparer : IComparer<Transaction>
    {
        public int Compare(Transaction? x, Transaction? y)
        {
            if (ReferenceEquals(x, y))
            {
                return 0;
            }

            if (x is null)
            {
                return 1;
            }

            if (y is null)
            {
                return -1;
            }

            // Different currencies can't be compared directly, so order by currency code.
            if (!string.Equals(x.Amount.Currency, y.Amount.Currency, StringComparison.Ordinal))
            {
                return string.Compare(x.Amount.Currency, y.Amount.Currency, StringComparison.Ordinal);
            }

            return y.Amount.Amount.CompareTo(x.Amount.Amount);
        }
    }
}
