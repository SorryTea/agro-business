using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    public class Sensor : ITickable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = "Sensor Wilgotności i UV";

        public double CriticalThreshold { get; set; } = 20.0;

        public double WaterReading { get; private set; }
        public double UVReading { get; private set; }

        public void Tick(FarmState state)
        {
            WaterReading = state.SoilMoisture;
            UVReading = state.LightLevel;

            if (WaterReading < CriticalThreshold)
            {
                foreach (var sprinkler in state.Sprinklers)
                {
                    sprinkler.IsOn = true;
                }

                // Throttle the alarm log so it doesn't spam every tick.
                if (state.CurrentTick % 5 == 0)
                {
                    state.Logger?.Invoke($"[agro.Core] Sensor: wykryto suszę ({WaterReading:F1}%). Uruchamiam zraszacze.");
                }
            }
            else
            {
                foreach (var sprinkler in state.Sprinklers)
                {
                    sprinkler.IsOn = false;
                }
            }
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
