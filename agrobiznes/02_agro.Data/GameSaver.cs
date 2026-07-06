using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using _01_agro.Core.Economy;
using _01_agro.Core;

namespace _02_agro.Data
{
    public enum LoadGameResult
    {
        Loaded,
        Missing,
        Failed
    }

    /// <summary>
    /// Saves and loads the simulation to and from a JSON file.
    /// </summary>
    public static class GameSaver
    {
        private static readonly string SaveDirectory =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Agrobiznes");

        private static readonly string FilePath = Path.Combine(SaveDirectory, "savegame.json");

        private static void SyncFinanceSnapshot(FarmState state)
        {
            state.BalanceAmount = state.Finance.Account.Balance.Amount;
            state.BalanceCurrency = state.Finance.Account.Balance.Currency;
            state.Transactions = state.Finance.Transactions.ToList();
        }

        private static void RestoreFinanceSnapshot(FarmState state)
        {
            if (string.IsNullOrWhiteSpace(state.BalanceCurrency))
            {
                state.BalanceCurrency = "PLN";
            }

            state.Transactions ??= new List<Transaction>();
            state.Finance = new FinanceEngine(
                new Account(new Money(state.BalanceAmount, state.BalanceCurrency)),
                new NoTax());
            state.Finance.RestoreTransactions(state.Transactions);
        }
        public static void SaveGame(FarmState state)
        {
            Directory.CreateDirectory(SaveDirectory);

            SyncFinanceSnapshot(state);

            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string jsonString = JsonSerializer.Serialize(state, options);

            string tempPath = FilePath + ".tmp";
            string backupPath = FilePath + ".bak";

            File.WriteAllText(tempPath, jsonString);

            if (File.Exists(FilePath))
            {
                File.Replace(tempPath, FilePath, backupPath);
            }
            else
            {
                File.Move(tempPath, FilePath);
            }
        }

        public static LoadGameResult TryLoadGame(out FarmState? state)
        {
            state = null;

            if (!File.Exists(FilePath))
            {
                return LoadGameResult.Missing;
            }
            try
            {
                string jsonString = File.ReadAllText(FilePath);
                state = JsonSerializer.Deserialize<FarmState>(jsonString);
                if (state != null)
                {
                    RestoreFinanceSnapshot(state);
                }
                return state != null ? LoadGameResult.Loaded : LoadGameResult.Failed;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LOAD ERROR]: Failed to load save file. {ex.Message}");
                return LoadGameResult.Failed;
            }
        }
    }
}
