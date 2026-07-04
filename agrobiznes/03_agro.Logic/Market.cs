using System;
using System.Collections.Generic;
using System.Linq;
using _01_agro.Core;
using _01_agro.Core.Economy;
using _02_agro.Data;


namespace _03_agro.Logic
{
    /// <summary>
    /// The in-game market: buying and selling of plants and machines.
    /// </summary>
    public class Market
    {
        private readonly FarmState _state;
        private readonly LogRepo _logger;

        public Market(FarmState state, LogRepo logger)
        {
            _state = state;
            _logger = logger;
        }

        private bool TryPay(float cost, TransactionCategory category, string description, out string message)
        {
            var costMoney = new Money((decimal)cost, "PLN");

            try
            {
                _state.Finance.Apply(new PurchaseTransaction(
                    costMoney,
                    category,
                    description
                ));

                message = string.Empty;
                return true;
            }
            catch (InvalidOperationException)
            {
                message = $"BŁĄD: Brak środków. Koszt: {costMoney}, saldo: {_state.Finance.Account.Balance}";
                _logger.AddLog(message);
                return false;
            }
        }
        public bool TryBuyPlant(float cost, string plantName, out string message)
        {
            return TryPay(cost, TransactionCategory.Other, $"Zakup rośliny: {plantName}", out message);
        }

        public string BuyTomatoes(int quantity)
        {
            // Build a prototype to read the current in-season price.
            var prototype = new Tomato();
            float cost = prototype.Price * quantity;

            if (!TryPay(cost, TransactionCategory.Seeds, $"Zakup: Pomidory x{quantity}", out var err))
            {
                return err;
            }


            for (int i = 0; i < quantity; i++)
            {
                var t = new Tomato();
                _state.Tomatoes.Add(t);
            }

            string msg = $"SKLEP: Kupiono {quantity} pomidorów. Koszt: {cost:C}";
            _logger.AddLog(msg);
            return msg;
        }

        public string BuyApples(int quantity)
        {
            var prototype = new Apple();
            float cost = prototype.Price * quantity;

            if (!TryPay(cost, TransactionCategory.Seeds, $"Zakup: Jabłka x{quantity}", out var err))
            {
                return err;
            }

            for (int i = 0; i < quantity; i++)
            {
                var t = new Apple();

                _state.Apples.Add(t);
            }
            string msg = $"SKLEP: Kupiono {quantity} jabłek. Koszt: {cost:C}";
            _logger.AddLog(msg);
            return msg;
        }

        public string BuyCacti(int quantity)
        {
            var prototype = new Cactus();
            float cost = prototype.Price * quantity;

            if (!TryPay(cost, TransactionCategory.Seeds, $"Zakup: Kaktusy x{quantity}", out var err))
            {
                return err;
            }

            for (int i = 0; i < quantity; i++)
            {
                var t = new Cactus();

                _state.Cactile.Add(t);
            }
            string msg = $"SKLEP: Kupiono {quantity} kaktusów. Koszt: {cost:C}";
            _logger.AddLog(msg);
            return msg;
        }

        public string BuyRoses(int quantity)
        {
            var prototype = new Rose();
            float cost = prototype.Price * quantity;

            if (!TryPay(cost, TransactionCategory.Seeds, $"Zakup: Róży x{quantity}", out var err))
            {
                return err;
            }

            for (int i = 0; i < quantity; i++)
            {
                var t = new Rose();

                _state.Roses.Add(t);
            }
            string msg = $"SKLEP: Kupiono {quantity} róż. Koszt: {cost:C}";
            _logger.AddLog(msg);
            return msg;
        }

        public string SellAll()
        {
            float totalEarnings = 0;
            int totalCount = 0;



            var tomatoResult = SellFromList(_state.Tomatoes);
            totalEarnings += tomatoResult.earnings;
            totalCount += tomatoResult.count;

            var appleResult = SellFromList(_state.Apples);
            totalEarnings += appleResult.earnings;
            totalCount += appleResult.count;

            var cactusResult = SellFromList(_state.Cactile);
            totalEarnings += cactusResult.earnings;
            totalCount += cactusResult.count;

            var roseResult = SellFromList(_state.Roses);
            totalEarnings += roseResult.earnings;
            totalCount += roseResult.count;

            if (totalCount > 0)
            {
                var revenueMoney = new Money((decimal)totalEarnings, "PLN");
                _state.Finance.Apply(new SaleTransaction(
                    revenueMoney,
                    TransactionCategory.Sales,
                    $"Sprzedaż roślin: {totalCount} szt."
                ));

                string msg = $"SKUP: Sprzedano {totalCount} roślin za {totalEarnings:C}.";
                _logger.AddLog(msg);
                return msg;
            }

            return "SKUP: Magazyn pusty (brak dojrzałych roślin).";
        }
        public bool TrySellAt(int row, int col, out string message)
        {
            Tomato tomato = _state.Tomatoes.FirstOrDefault(p => p.Row == row && p.Col == col);
            if (tomato != null)
            {
                return SellSpecific(_state.Tomatoes, tomato, row, col, out message);
            }

            Apple apple = _state.Apples.FirstOrDefault(p => p.Row == row && p.Col == col);
            if (apple != null)
            {
                return SellSpecific(_state.Apples, apple, row, col, out message);
            }

            Cactus cactus = _state.Cactile.FirstOrDefault(p => p.Row == row && p.Col == col);
            if (cactus != null)
            {
                return SellSpecific(_state.Cactile, cactus, row, col, out message);
            }

            Rose rose = _state.Roses.FirstOrDefault(p => p.Row == row && p.Col == col);
            if (rose != null)
            {
                return SellSpecific(_state.Roses, rose, row, col, out message);
            }

            message = $"SKUP: Pole ({row},{col}) jest puste.";
            return false;
        }

        private bool SellSpecific<T>(List<T> list, T plant, int row, int col, out string message)
            where T : Plant
        {
            if (!plant.IsMature || plant.IsDead)
            {
                message = $"SKUP: Roślina na ({row},{col}) nie jest gotowa do sprzedaży.";
                return false;
            }

            float income = plant.SalePrice;

            list.Remove(plant);

            var revenueMoney = new Money((decimal)income, "PLN");
            _state.Finance.Apply(new SaleTransaction(
                revenueMoney,
                TransactionCategory.Sales,
                $"Sprzedaż rośliny na polu ({row},{col})"
            ));

            message = $"SKUP: Sprzedano roślinę z pola ({row},{col}) za {income:C}.";
            _logger.AddLog(message);
            return true;
        }

        private (int count, float earnings) SellFromList<T>(List<T> plantList) where T : Plant
        {
            var toSell = plantList.Where(r => r.IsMature && !r.IsDead).ToList();

            if (toSell.Count == 0)
            {
                return (0, 0);
            }

            float profit = toSell.Sum(r => r.SalePrice);

            plantList.RemoveAll(r => r.IsMature && !r.IsDead);

            return (toSell.Count, profit);
        }

        public string BuySprinkler()
        {
            var newMachine = new Sprinkler();
            return BuyMachine(newMachine, _state.Sprinklers);
        }

        public string BuySolarPanel()
        {
            var newMachine = new Solar();
            return BuyMachine(newMachine, _state.Solars);
        }

        private string BuyMachine<T>(T machine, List<T> targetList) where T : Device
        {
            float cost = machine.Price;

            if (!TryPay(cost, TransactionCategory.Other, $"Zakup maszyny: {machine.Name}", out var err))
            {
                return err;
            }

            targetList.Add(machine);

            string msg = $"SKLEP: Zakupiono maszynę: {machine.Name}. Koszt: {cost:C}";
            _logger.AddLog(msg);
            return msg;
        }
    }
}
