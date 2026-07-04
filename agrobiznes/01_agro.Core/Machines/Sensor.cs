using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    /// <summary>
    /// Klasa Sensor, która również będzie odświeżana poprzez interfejs ITickable
    /// </summary>
    public class Sensor : ITickable
    {
        [Key] // Klucz główny w bazie
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Używamy własnego GUID, nie auto-number
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "Sensor Wilgotności i UV";

        // Ustawienia sensora
        public double CriticalThreshold { get; set; } = 20.0; // Poniżej 20% włącza alarm

        // Ostatni odczyt (żeby np. GUI mogło go wyświetlić)
        public double WaterReading { get; private set; }
        public double UVReading { get; private set; }

        public void Tick(FarmState state)
        {
            // 1. POBIERZ DANE (READ)
            // Sensor "patrzy" na glebę w FarmState
            WaterReading = state.SoilMoisture;
            UVReading = state.LightLevel;

            // 2a. ANALIZA I ALARM: Woda
            if (WaterReading < CriticalThreshold)
            {
                // Jest sucho -> Włączamy WSZYSTKIE zraszacze
                foreach (var sprinkler in state.Sprinklers)
                {
                    sprinkler.IsOn = true;
                }

                // Logujemy alarm raz na jakiś czas (żeby nie spamować co sekundę)
                if (state.CurrentTick % 5 == 0)
                {
                    state.Logger?.Invoke($"[agro.Core] Sensor: wykryto suszę ({WaterReading:F1}%). Uruchamiam zraszacze.");
                }
            }
            else
            {
                // Jest mokro -> Wyłączamy zraszacze (oszczędzamy wodę/zasoby)
                foreach (var sprinkler in state.Sprinklers)
                {
                    sprinkler.IsOn = false;
                }
            }
            // 2b. ANALIZA I ALARM: UV
            if (UVReading < CriticalThreshold)
            {
                foreach (var lamp in state.Solars)
                {
                    lamp.IsOn = true;
                }

                if (state.CurrentTick % 5 == 0 && state.Solars.Count > 0)
                {
                    state.Logger?.Invoke($"[agro.Core] Sensor: ciemno ({UVReading}%). Włączam lampy UV.");
                }
            }
            else
            {
                foreach (var lamp in state.Solars)
                {
                    lamp.IsOn = false;
                }
            }
        }
    }
}
