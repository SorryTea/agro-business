using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Obiekt wartości reprezentujący kwotę pieniężną.
    /// Używany w całym module ekonomii (saldo, transakcje, raporty).
    /// </summary>
    [ComplexType]
    public class Money: IEquatable<Money>
    {
        [JsonInclude]
        public decimal Amount { get; set; }
        [JsonInclude]
        public string Currency { get; set; }

  
        public Money(decimal amount, string currency)
        {
            if (amount < 0m)
                throw new System.ArgumentOutOfRangeException(nameof(amount));
            Amount = amount;
            Currency = currency;
        }
        public Money(decimal amount) : this(amount, "PLN") { }

        public Money() { }

        public override string ToString()
        {
            return $"{Amount:0.00} {Currency}";
        }

        // ===== IEquatable =====
        public bool Equals(Money? other)
        {
            if (ReferenceEquals(other, null)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Amount == other.Amount &&
                   string.Equals(Currency, other.Currency, StringComparison.Ordinal);
        }

        public override bool Equals(object? obj) => Equals(obj as Money);

        public override int GetHashCode()
            => HashCode.Combine(Amount, Currency);

        public static bool operator ==(Money? left, Money? right)
            => Equals(left, right);

        public static bool operator !=(Money? left, Money? right)
            => !Equals(left, right);
    }
}
