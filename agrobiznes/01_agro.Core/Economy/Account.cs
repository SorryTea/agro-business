using System;
using System.Text.Json.Serialization;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Financial account. Holds the farm's current balance.
    /// </summary>

    public class Account
    {

        [JsonInclude]
        public Money Balance { get; private set; }

        public event Action<Money>? BalanceChanged;


        public Account(Money initialBalance)
        {
            Balance = initialBalance;
        }

        // required for System.Text.Json deserialization
        public Account()
        {
            Balance = new Money(0);
        }

        public void Credit(Money amount)
        {
            Balance = new Money(Balance.Amount + amount.Amount, Balance.Currency);
            BalanceChanged?.Invoke(Balance);
        }

        public void Debit(Money amount)
        {
            if (amount.Amount > Balance.Amount)
            {
                throw new InvalidOperationException("Brak środków");
            }

            Balance = new Money(Balance.Amount - amount.Amount, Balance.Currency);
            BalanceChanged?.Invoke(Balance);
        }
    }
}
