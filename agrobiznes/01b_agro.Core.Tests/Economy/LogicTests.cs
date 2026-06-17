using _01_agro.Core;
using _03_agro.Logic;

namespace _01b_agro.Core.Tests;

[TestClass]
public class LogicTests
{

    public TestContext TestContext { get; set; }
    [TestMethod]
    public void Billing_Should_Charge_Costs_Every_30_Ticks()
    {
        var engine = new SimulationEngine();


        if (engine.State.Sprinklers.Count == 0)
        {
            engine.State.Sprinklers.Add(new Sprinkler { IsOn = true });
        }
        else
        {
            foreach (var s in engine.State.Sprinklers)
            {
                s.IsOn = true;
            }
        }

        if (engine.State.Solars.Count == 0)
        {
            engine.State.Solars.Add(new Solar { IsOn = true });
        }
        else
        {
            foreach (var l in engine.State.Solars)
            {
                l.IsOn = true;
            }
        }

        TestContext.WriteLine($"Sprinklers: {engine.State.Sprinklers.Count}, ON: {engine.State.Sprinklers.Count(s => s.IsOn)}");
        TestContext.WriteLine($"Solars: {engine.State.Solars.Count}, ON: {engine.State.Solars.Count(l => l.IsOn)}");

        engine.State.CurrentTick = 29;

        var before = engine.State.Finance.Account.Balance.Amount;

        engine.Tick();

        var after = engine.State.Finance.Account.Balance.Amount;

        TestContext.WriteLine($"Tick after: {engine.State.CurrentTick}");
        TestContext.WriteLine($"Before: {before}, After: {after}");

        Assert.IsLessThan(before, after, "Saldo powinno spaść po rozliczeniu kosztów na ticku 30.");
    }
}
