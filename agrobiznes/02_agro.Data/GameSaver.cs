using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using _01_agro.Core;

namespace _02_agro.Data
{
    /// <summary>
    /// Saves and loads the simulation to and from a JSON file.
    /// </summary>
    public static class GameSaver
    {
        private static readonly string FilePath = "savegame.json";

        public static void SaveGame(FarmState state)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };

            string jsonString = JsonSerializer.Serialize(state, options);
            File.WriteAllText(FilePath, jsonString);
        }

        public static FarmState LoadGame()
        {
            if (!File.Exists(FilePath))
            {
                return null;
            }
            try
            {
                string jsonString = File.ReadAllText(FilePath);

                var state = JsonSerializer.Deserialize<FarmState>(jsonString);

                return state;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LOAD ERROR]: Failed to load save file. {ex.Message}");
                return null;
            }

        }
    }
}
