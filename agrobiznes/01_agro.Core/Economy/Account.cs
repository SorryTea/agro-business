using System;
using System.Text.Json.Serialization;

namespace _01_agro.Core.Economy
{
    /// <summary>
    /// Konto finansowe systemu.
    /// Przechowuje aktualne saldo farmy.
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

        // Pusty konstruktor prywatny.
        // Jest niezbędny dla Entity Framework (baza) i pomaga przy deserializacji.
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
