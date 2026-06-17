using _01_agro.Core;
using _01_agro.Core.Economy;
using _02_agro.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace _03_agro.Logic
{
    /// <summary>
    /// Klasa SilnikSymulacji odpowiada za działanie środowiska symulacji
    /// </summary>
    public class SimulationEngine
    {
        private FarmState _state;
        private readonly LogRepo _logger;

        private const int BillingIntervalTicks = 30;

        public FarmState State => _state;
        public LogRepo LoggerRepo => _logger;

        private System.Timers.Timer? _gameTimer;

        public Market Market { get; private set; }
        public event Action<FarmState>? TickHappened;

        private readonly object _sync = new object();

        private bool _tickInProgress = false;

        // =========================
        // Finance
        // =========================
        private void EnsureFinanceInitialized()
        {
            if (_state.Finance == null)
            {
                _state.Finance = new FinanceEngine(
                    new Account(new Money(_state.BalanceAmount, _state.BalanceCurrency)),
                    new NoTax()
                );
            }
        }

        private void ApplyOperatingCostsIfDue()
        {
            if (_state.CurrentTick % BillingIntervalTicks != 0)
                return;

            int onSprinklers = _state.Sprinklers.Count(s => s.IsOn);
            int onLamps = _state.Solars.Count(l => l.IsOn);

            decimal waterCostPerDevice = 3.0m;
            decimal energyCostPerDevice = 5.0m;

            decimal waterTotal = onSprinklers * waterCostPerDevice;
            decimal energyTotal = onLamps * energyCostPerDevice;

            if (waterTotal > 0)
            {
                _state.Finance.Apply(new PurchaseTransaction(
                    new Money(waterTotal, "PLN"),
                    TransactionCategory.Water,
                    $"Koszty wody ({BillingIntervalTicks} ticków): {onSprinklers} aktywnych zraszaczy"
                ));
            }

            if (energyTotal > 0)
            {
                _state.Finance.Apply(new PurchaseTransaction(
                    new Money(energyTotal, "PLN"),
                    TransactionCategory.Energy,
                    $"Koszty energii ({BillingIntervalTicks} ticków): {onLamps} aktywnych lamp UV"
                ));
            }

            _state.Logger?.Invoke(
                $"[FINANSE] Rozliczono koszty operacyjne za {BillingIntervalTicks} ticków " +
                $"(woda: {waterTotal:0.00} PLN, energia: {energyTotal:0.00} PLN)."
            );
        }

        // =========================
        //  KONSTRUKTOR
        // =========================
        public SimulationEngine()
        {
            _logger = new LogRepo();

            var loaded = GameSaver.LoadGame();
            if (loaded == null)
            {
                _state = new FarmState();
                _logger.AddLog("Rozpoczęto nową grę.");
            }
            else
            {
                _state = loaded;
                _logger.AddLog("Wczytano zapis gry.");
            }

            EnsureFinanceInitialized();

            // Podpięcie loggera (żeby Core mógł logować)
            _state.Logger = (message) => _logger.AddLog(message);

            // Market inicjalizujemy raz
            Market = new Market(_state, _logger);

            // Startowy pakiet farmy (tylko jeśli pusto)
            InitializeStarterFarm();

            // Timer: tworzymy, ale odpalamy dopiero w StartSimulation()
            _gameTimer = new System.Timers.Timer(1000);
            _gameTimer.Elapsed += OnTimedEvent;
            _gameTimer.AutoReset = true;
        }

        // =========================
        //  START / STOP
        // =========================
        public void StartSimulation()
        {
            if (_gameTimer == null)
            {
                _gameTimer = new System.Timers.Timer(1000);
                _gameTimer.Elapsed += OnTimedEvent;
                _gameTimer.AutoReset = true;
            }

            _gameTimer.Start();
            _logger.AddLog("SYMULACJA: Rozpoczęto.");
        }

        public void StopSimulation()
        {
            try
            {
                if (_gameTimer != null)
                {
                    _gameTimer.Stop();
                    _gameTimer.Elapsed -= OnTimedEvent;
                    _gameTimer.Dispose();
                    _gameTimer = null;
                }
            }
            catch { /* ignorujemy */ }

            _logger.AddLog("SYMULACJA: Zatrzymano.");

            lock (_sync)
            {
                GameSaver.SaveGame(_state);
            }
        }

        // =========================
        //  TIMER CALLBACK
        // =========================
        private void OnTimedEvent(object? source, ElapsedEventArgs e)
        {
            // ochrona przed nakładaniem ticków
            lock (_sync)
            {
                if (_tickInProgress) return;
                _tickInProgress = true;
            }

            FarmState snapshot;

            try
            {
                // cały Tick + modyfikacje stanu w locku
                lock (_sync)
                {
                    Tick_Internal_NoEvent(); 
                    snapshot = _state;
                }
            }
            finally
            {
                lock (_sync) { _tickInProgress = false; }
            }

            TickHappened?.Invoke(snapshot);
        }

        // =========================
        //  Rejestracja obiektów
        // =========================
        public void RegisterObject(ITickable obj)
        {
            lock (_sync)
            {
                if (obj is Tomato tomato) _state.Tomatoes.Add(tomato);
                else if (obj is Apple apple) _state.Apples.Add(apple);
                else if (obj is Rose rose) _state.Roses.Add(rose);
                else if (obj is Cactus cactus) _state.Cactile.Add(cactus);
                else if (obj is Sprinkler sprinkler) _state.Sprinklers.Add(sprinkler);
                else if (obj is Solar solar) _state.Solars.Add(solar);
                else if (obj is Sensor sensor) _state.Sensors.Add(sensor);
                else
                {
                    _logger.AddLog($"BŁĄD: Nieznany typ obiektu: {obj.GetType().Name}");
                    return;
                }

                _logger.AddLog($"[agro.Logic]: Dodano nowy obiekt: {obj.GetType().Name}");
            }
        }

        // =========================
        //  Pakiet startowy
        // =========================
        public void InitializeStarterFarm()
        {
            lock (_sync)
            {
                if (_state.Tomatoes.Count == 0 &&
                    _state.Roses.Count == 0 &&
                    _state.Cactile.Count == 0 &&
                    _state.Apples.Count == 0)
                {
                    _state.SoilMoisture = 100;
                    Random rnd = new Random();
                    _state.LightLevel = rnd.Next(30, 90);

                    _logger.AddLog("[agro.Logic]: Wykryto pustą farmę. Tworzenie pakietu startowego...");

                    for (int i = 0; i < 40; i++)
                    {
                        var p = new Tomato
                        {
                            PoziomNawodnienia = 60,
                            PoziomNaslonecznienia = 60,
                            PoziomWzrostu = 0
                        };
                        _state.Tomatoes.Add(p);
                    }

                    for (int i = 0; i < 5; i++) _state.Sprinklers.Add(new Sprinkler { IsOn = false });
                    for (int i = 0; i < 3; i++) _state.Solars.Add(new Solar { IsOn = false });
                    _state.Sensors.Add(new Sensor());

                    GameSaver.SaveGame(_state);
                }
            }
        }

        // =========================
        //  Tick publiczny (jeśli kiedyś chcesz ręczne ticki)
        // =========================
        public void Tick()
        {
            lock (_sync)
            {
                Tick_Internal_NoEvent();
            }

        }

        // =========================
        //  GŁÓWNY TICK (bez eventu)
        // =========================
        private void Tick_Internal_NoEvent()
        {
            _state.CurrentTick++;

            // gleba + UV
            _state.SoilMoisture -= 0.1 * (_state.Tomatoes.Count + _state.Roses.Count + _state.Apples.Count);
            if (_state.SoilMoisture < 0) _state.SoilMoisture = 0;

            _state.LightLevel -= 1.0;
            if (_state.LightLevel < 0) _state.LightLevel = 0;

            // zbieramy wszystko do jednej listy
            var allObjects = new List<ITickable>();
            allObjects.AddRange(_state.Sprinklers);
            allObjects.AddRange(_state.Solars);
            allObjects.AddRange(_state.Sensors);
            allObjects.AddRange(_state.Tomatoes);
            allObjects.AddRange(_state.Apples);
            allObjects.AddRange(_state.Roses);
            allObjects.AddRange(_state.Cactile);

            ApplyOperatingCostsIfDue();

            foreach (var obj in allObjects)
            {
                try
                {
                    obj.Tick(_state);
                }
                catch (Exception ex)
                {
                    _state.Logger?.Invoke($"Błąd obiektu {obj.GetType().Name}: {ex.Message}");
                }
            }

            // aktualizacja salda w FarmState
            _state.BalanceAmount = _state.Finance.Account.Balance.Amount;
            _state.BalanceCurrency = _state.Finance.Account.Balance.Currency;

            // autosave co 10 tur
            if (_state.CurrentTick % 10 == 0)
            {
                try
                {
                    GameSaver.SaveGame(_state);
                }
                catch (Exception ex)
                {
                    _logger.AddLog($"[agro.Logic] Błąd zapisu: {ex.Message}");
                }
            }

            // sprzątanie martwych roślin
            _state.Tomatoes.RemoveAll(p => p.IsDead);
            _state.Cactile.RemoveAll(p => p.IsDead);
            _state.Roses.RemoveAll(p => p.IsDead);
            _state.Apples.RemoveAll(p => p.IsDead);
        }

        // =========================
        //  Sadzenie i sprawdzanie zajętości
        // =========================
        public bool PlantAt(int row, int col, string plantType)
        {
            lock (_sync)
            {
                if (IsOccupied_NoLock(row, col))
                {
                    _logger.AddLog($"[ENGINE] Pole ({row},{col}) zajęte – nie można posadzić {plantType}.");
                    return false;
                }

                ITickable plant = plantType switch
                {
                    "Tomato" => new Tomato(),
                    "Rose" => new Rose(),
                    "Cactus" => new Cactus(),
                    _ => new Tomato()
                };

                if (plant is IPositioned positioned)
                {
                    positioned.Row = row;
                    positioned.Col = col;
                }

                RegisterObject(plant);
                _logger.AddLog($"[ENGINE] Posadzono {plantType} na ({row},{col}).");
                return true;
            }
        }

        public bool IsOccupied(int row, int col)
        {
            lock (_sync)
            {
                return IsOccupied_NoLock(row, col);
            }
        }

        private bool IsOccupied_NoLock(int row, int col)
        {
            bool HasAt<T>(IEnumerable<T> list) where T : class
                => list.OfType<IPositioned>().Any(p => p.Row == row && p.Col == col);

            return HasAt(_state.Tomatoes)
                || HasAt(_state.Roses)
                || HasAt(_state.Cactile)
                || HasAt(_state.Apples);
        }
    }
}
