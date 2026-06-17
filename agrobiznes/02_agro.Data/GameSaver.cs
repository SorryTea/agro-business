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
    /// Klasa ZapisGry pozawala na zapisywanie i wczytywanie symulacji z pliku JSON
    /// </summary>
    public static class GameSaver
    {
        // Plik zapisu w folderze bin/Debug projektu
        private static readonly string FilePath = "savegame.json";

        public static void SaveGame(FarmState state)
        {
            // Opcja WriteIndented = true sprawia, że plik JSON jest ładnie sformatowany (czytelny dla człowieka)
            var options = new JsonSerializerOptions { WriteIndented = true };

            string jsonString = JsonSerializer.Serialize(state, options);
            File.WriteAllText(FilePath, jsonString);
        }

        public static FarmState LoadGame()
        {
            if (!File.Exists(FilePath))
            {
                return null; // Brak pliku = brak zapisu
            }
            try
            {
                string jsonString = File.ReadAllText(FilePath);

                // Tutaj magia dzieje się sama - JSON widzi listę Tomatoes i ładuje do niej pomidory
                var state = JsonSerializer.Deserialize<FarmState>(jsonString);

                return state;
            }
            catch(Exception ex) 
            {
                File.Delete(FilePath);
                System.Diagnostics.Debug.WriteLine($"[BŁĄD ZAPISU]: Nie udało się wczytać gry. {ex.Message}");
                return null;
            }
            
        }
    }
}
