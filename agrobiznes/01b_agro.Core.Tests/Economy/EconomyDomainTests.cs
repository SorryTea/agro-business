using _01_agro.Core.Economy;

namespace _01b_agro.Core.Tests;

[TestClass]
public class MoneyTests
{
    [TestMethod]
    public void Money_Should_Set_Currency_From_Constructor()
    {
        var m = new Money(10m, "EUR");
        Assert.AreEqual("EUR", m.Currency);
    }

    [TestMethod]
    public void Money_Should_Default_To_PLN()
    {
        var m = new Money(10m);
        Assert.AreEqual("PLN", m.Currency);
    }

    [TestMethod]
    public void Money_Should_Throw_When_Amount_Is_Negative()
    {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Money(-1m, "PLN"));
    }

    [TestMethod]
    public void Money_Equals_Should_Work_For_Same_Value()
    {
        var a = new Money(10m, "PLN");
        var b = new Money(10m, "PLN");

        Assert.IsTrue(a.Equals(b));
        Assert.IsTrue(a == b);
    }

}

[TestClass]
public class AccountTests
{
    [TestMethod]
    public void Credit_Should_Increase_Balance()
    {
        var acc = new Account(new Money(100m));
        acc.Credit(new Money(25m));

        Assert.AreEqual(125m, acc.Balance.Amount);
        Assert.AreEqual("PLN", acc.Balance.Currency);
    }

    [TestMethod]
    public void Debit_Should_Decrease_Balance()
    {
        var acc = new Account(new Money(100m));
        acc.Debit(new Money(40m));

        Assert.AreEqual(60m, acc.Balance.Amount);
    }

    [TestMethod]
    public void Debit_Should_Throw_When_Insufficient_Funds()
    {
        var acc = new Account(new Money(50m));
        Assert.Throws<InvalidOperationException>(() => acc.Debit(new Money(60m)));
    }

    [TestMethod]
    public void BalanceChanged_Should_Fire_On_Credit_And_Debit()
    {
        var acc = new Account(new Money(100m));
        var fired = 0;

        acc.BalanceChanged += _ => fired++;

        acc.Credit(new Money(10m));
        acc.Debit(new Money(5m));

        Assert.AreEqual(2, fired);
    }
}

[TestClass]
public class TransactionTests
{
    [TestMethod]
    public void SaleTransaction_Should_Credit_Account()
    {
        var acc = new Account(new Money(100m));
        var tx = new SaleTransaction(new Money(30m), TransactionCategory.Sales, "Sprzedaż");

        tx.Apply(acc);

        Assert.AreEqual(130m, acc.Balance.Amount);
        Assert.AreEqual(TransactionType.Sale, tx.Type);
    }

    [TestMethod]
    public void PurchaseTransaction_Should_Debit_Account()
    {
        var acc = new Account(new Money(100m));
        var tx = new PurchaseTransaction(new Money(30m), TransactionCategory.Energy, "Prąd");

        tx.Apply(acc);

        Assert.AreEqual(70m, acc.Balance.Amount);
        Assert.AreEqual(TransactionType.Purchase, tx.Type);
    }

    [TestMethod]
    public void PenaltyTransaction_Should_Debit_Account()
    {
        var acc = new Account(new Money(100m));
        var tx = new PenaltyTransaction(new Money(15m), TransactionCategory.Fine, "Kara");

        tx.Apply(acc);

        Assert.AreEqual(85m, acc.Balance.Amount);
        Assert.AreEqual(TransactionType.Penalty, tx.Type);
    }

    [TestMethod]
    public void TransactionComparer_Should_Sort_By_Amount_Desc()
    {
        var list = new List<Transaction>
        {
            new SaleTransaction(new Money(10m, "PLN"), TransactionCategory.Sales, "a"),
            new SaleTransaction(new Money(50m, "PLN"), TransactionCategory.Sales, "b"),
            new SaleTransaction(new Money(20m, "PLN"), TransactionCategory.Sales, "c"),
        };

        list.Sort(new TransactionByAmountDescComparer());

        Assert.AreEqual(50m, list[0].Amount.Amount);
        Assert.AreEqual(20m, list[1].Amount.Amount);
        Assert.AreEqual(10m, list[2].Amount.Amount);
    }

}

[TestClass]
public class TaxTests
{
    [TestMethod]
    public void NoTax_Should_Return_Zero()
    {
        var tax = new NoTax();
        var period = new FinancialPeriod
        {
            Profit = new Money(100m, "PLN"),
            Revenue = new Money(200m, "PLN"),
            Costs = new Money(100m, "PLN"),
            From = DateTimeOffset.UtcNow.AddDays(-1),
            To = DateTimeOffset.UtcNow
        };

        var result = tax.CalculateTax(period);

        Assert.AreEqual(0m, result.Amount);
        Assert.AreEqual("PLN", result.Currency);
    }

    [TestMethod]
    public void FlatTax_Should_Calculate_Tax_From_Profit()
    {
        var tax = new FlatTax(0.19m);
        var period = new FinancialPeriod
        {
            Profit = new Money(100m, "PLN"),
            Revenue = new Money(200m, "PLN"),
            Costs = new Money(100m, "PLN"),
            From = DateTimeOffset.UtcNow.AddDays(-1),
            To = DateTimeOffset.UtcNow
        };

        var result = tax.CalculateTax(period);

        Assert.AreEqual(19m, result.Amount);
        Assert.AreEqual("PLN", result.Currency);
    }

    [TestMethod]
    public void FlatTax_Should_Return_Zero_When_Profit_Is_NonPositive()
    {
        var tax = new FlatTax(0.19m);
        var period = new FinancialPeriod
        {
            Profit = new Money(0m, "PLN"),
            Revenue = new Money(100m, "PLN"),
            Costs = new Money(120m, "PLN"),
            From = DateTimeOffset.UtcNow.AddDays(-1),
            To = DateTimeOffset.UtcNow
        };

        var result = tax.CalculateTax(period);
        Assert.AreEqual(0m, result.Amount);
    }

    [TestMethod]
    public void FlatTax_Should_Throw_When_Rate_Is_Out_Of_Range()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new FlatTax(1.5m));
    }

    [TestClass]
    public class FinanceEngineTests
    {
        [TestMethod]
        public void Apply_Should_Update_Balance_And_Add_Transaction()
        {
            var acc = new Account(new Money(100m));
            var engine = new FinanceEngine(acc, new NoTax());

            engine.Apply(new PurchaseTransaction(new Money(30m), TransactionCategory.Energy, "Prąd"));

            Assert.AreEqual(70m, engine.Account.Balance.Amount);
            Assert.AreEqual(1, engine.Transactions.Count);
        }

        [TestMethod]
        public void GetReport_Should_Return_Correct_Sums()
        {
            var now = DateTimeOffset.UtcNow;

            var acc = new Account(new Money(0m));
            var engine = new FinanceEngine(acc, new NoTax());

            engine.Apply(new SaleTransaction(new Money(200m), TransactionCategory.Sales, "Sprzedaż", now));
            engine.Apply(new PurchaseTransaction(new Money(50m), TransactionCategory.Energy, "Prąd", now));

            var report = engine.GetReport(now.AddMinutes(-1), now.AddMinutes(1));

            Assert.AreEqual(200m, report.Revenue.Amount);
            Assert.AreEqual(50m, report.Costs.Amount);
            Assert.AreEqual(150m, report.Profit.Amount);
            Assert.AreEqual(0m, report.Tax.Amount);
            Assert.AreEqual(150m, report.NetProfit.Amount);
        }
    }
}
