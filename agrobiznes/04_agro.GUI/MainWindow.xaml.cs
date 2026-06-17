using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _03_agro.Logic;
using _01_agro.Core;
using _01_agro.Core.Economy;

namespace _04_agro.GUI
{
    public partial class MainWindow : Window
    {
        private const int Rows = 10;
        private const int Cols = 10;

        private SimulationEngine _engine;
        private (int row, int col)? _selectedField;

        private readonly Dictionary<(int row, int col), Button> _cellButtons = new();

        private DateTime _lastLogRefresh = DateTime.MinValue;
        private static readonly TimeSpan LogRefreshInterval = TimeSpan.FromSeconds(1.5);
        private int _lastLogCount = -1;

        private DateTime _lastGridRefresh = DateTime.MinValue;
        private static readonly TimeSpan GridRefreshInterval = TimeSpan.FromMilliseconds(250);

        private static readonly Brush NormalBorderBrush = BrushFromHex("#5A5A5A");
        private static readonly Thickness NormalBorderThickness = new Thickness(1);
        private static readonly Brush SelectedBorderBrush = BrushFromHex("#9CDCFE");
        private static readonly Thickness SelectedBorderThickness = new Thickness(2);

        private enum PlantType { Tomato, Rose, Cactus }

        public MainWindow()
        {
            InitializeComponent();

            InitializeFarmGrid();

            _engine = new SimulationEngine();
            _engine.TickHappened += OnEngineTick;
            _engine.StartSimulation();

            _engine.LoggerRepo.AddLog("[GUI] Uruchomiono MainWindow.");

            RenderAll(_engine.State);
            LoadLogsFromEngine(force: true);
        }

        // =========================
        //  GRID
        // =========================
        private void InitializeFarmGrid()
        {
            FarmGrid.Children.Clear();
            _cellButtons.Clear();

            for (int row = 0; row < Rows; row++)
                for (int col = 0; col < Cols; col++)
                {
                    var btn = new Button
                    {
                        Content = "",
                        Tag = (row, col),
                        Margin = new Thickness(2),
                        Background = BrushFromHex("#252526"),
                        BorderBrush = NormalBorderBrush,
                        BorderThickness = NormalBorderThickness
                    };

                    btn.Click += FarmField_Click;

                    FarmGrid.Children.Add(btn);
                    _cellButtons[(row, col)] = btn;
                }
        }

        private void FarmField_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button button) return;
            _selectedField = ((int, int))button.Tag;

            RefreshSelectionVisual();

            var (r, c) = _selectedField.Value;
            _engine.LoggerRepo.AddLog($"[GUI] Zaznaczono pole ({r},{c}).");
        }

        private void RefreshSelectionVisual()
        {
            foreach (var child in FarmGrid.Children)
            {
                if (child is not Button btn) continue;

                btn.BorderBrush = NormalBorderBrush;
                btn.BorderThickness = NormalBorderThickness;

                if (_selectedField.HasValue)
                {
                    var (r, c) = ((int, int))btn.Tag;
                    if (r == _selectedField.Value.row && c == _selectedField.Value.col)
                    {
                        btn.BorderBrush = SelectedBorderBrush;
                        btn.BorderThickness = SelectedBorderThickness;
                    }
                }
            }
        }

        // =========================
        //  NEW GAME
        // =========================
        private void NewGame_Click(object sender, RoutedEventArgs e)
        {
            var res = MessageBox.Show(
                "Czy na pewno rozpocząć nową grę?\nObecny zapis zostanie usunięty.",
                "Nowa gra",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (res != MessageBoxResult.Yes) return;

            try
            {
                _engine?.StopSimulation();

                const string savePath = "savegame.json";
                if (System.IO.File.Exists(savePath))
                    System.IO.File.Delete(savePath);

                _selectedField = null;
                RefreshSelectionVisual();

                _engine = new SimulationEngine();
                _engine.TickHappened += OnEngineTick;
                _engine.StartSimulation();

                _engine.LoggerRepo.AddLog("[GUI] NOWA GRA.");

                RenderAll(_engine.State);
                LoadLogsFromEngine(force: true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Błąd podczas tworzenia nowej gry: " + ex.Message);
            }
        }

        // =========================
        //  MANUAL TICK (uses SimulationEngine.Tick())
        // =========================
        private void ManualTick_Click(object sender, RoutedEventArgs e)
        {
            _engine.Tick();                 
            RenderAll(_engine.State);
            LoadLogsFromEngine(force: false);
        }

        // =========================
        //  BUY + PLANT (uses Market.TryBuyPlant + Engine.PlantAt/IsOccupied)
        // =========================
        private void BuyTomato_Click(object sender, RoutedEventArgs e) => BuySelectedField(PlantType.Tomato);
        private void BuyRose_Click(object sender, RoutedEventArgs e) => BuySelectedField(PlantType.Rose);
        private void BuyCactus_Click(object sender, RoutedEventArgs e) => BuySelectedField(PlantType.Cactus);

        private void BuySelectedField(PlantType type)
        {
            if (_selectedField == null)
            {
                _engine.LoggerRepo.AddLog("[GUI] Najpierw zaznacz pole.");
                return;
            }

            var (row, col) = _selectedField.Value;

            if (_engine.IsOccupied(row, col))
            {
                _engine.LoggerRepo.AddLog($"[GUI] Pole ({row},{col}) jest zajęte.");
                return;
            }

            float cost = type switch
            {
                PlantType.Tomato => 50f,
                PlantType.Rose => 80f,
                PlantType.Cactus => 40f,
                _ => 50f
            };

            string plantName = type.ToString();

            if (!_engine.Market.TryBuyPlant(cost, plantName, out string msg)) 
            {
                _engine.LoggerRepo.AddLog($"[GUI] Zakup nieudany: {msg}");
                SystemSounds.Hand.Play();
                return;
            }

            if (!_engine.PlantAt(row, col, plantName)) 
                return;

            SystemSounds.Asterisk.Play();
            RenderAll(_engine.State);
            LoadLogsFromEngine(force: false);
        }

        // =========================
        //  SELL (uses Market.TrySellAt)
        // =========================
        private void Sell_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedField == null)
            {
                _engine.LoggerRepo.AddLog("[GUI] Sprzedaj: najpierw zaznacz pole.");
                return;
            }

            var (row, col) = _selectedField.Value;

            if (_engine.Market.TrySellAt(row, col, out string msg)) 
            {
                _engine.LoggerRepo.AddLog(msg);
                SystemSounds.Asterisk.Play();
                RenderAll(_engine.State);
                LoadLogsFromEngine(force: false);
                return;
            }

            _engine.LoggerRepo.AddLog(msg);
            SystemSounds.Beep.Play();
            LoadLogsFromEngine(force: false);
        }

        // =========================
        //  SHOP STOCK (uses Market.KupPomidory/Jablka/Kaktusy/Róże)
        // =========================
        private int ReadQty()
        {
            if (int.TryParse(QtyBox.Text, out var qty) && qty > 0) return qty;
            return 1;
        }

        private void BuyTomatoesStock_Click(object sender, RoutedEventArgs e)
        {
            var msg = _engine.Market.KupPomidory(ReadQty());
            _engine.LoggerRepo.AddLog(msg);
            SystemSounds.Asterisk.Play();
            RenderAll(_engine.State);
            LoadLogsFromEngine(false);
        }

        private void BuyApplesStock_Click(object sender, RoutedEventArgs e)
        {
            var msg = _engine.Market.KupJablka(ReadQty());
            _engine.LoggerRepo.AddLog(msg);
            SystemSounds.Asterisk.Play();
            RenderAll(_engine.State);
            LoadLogsFromEngine(false);
        }

        private void BuyCactusesStock_Click(object sender, RoutedEventArgs e)
        {
            var msg = _engine.Market.KupKaktusy(ReadQty());
            _engine.LoggerRepo.AddLog(msg);
            SystemSounds.Asterisk.Play();
            RenderAll(_engine.State);
            LoadLogsFromEngine(false);
        }

        private void BuyRosesStock_Click(object sender, RoutedEventArgs e)
        {
            var msg = _engine.Market.KupRóże(ReadQty());
            _engine.LoggerRepo.AddLog(msg);
            SystemSounds.Asterisk.Play();
            RenderAll(_engine.State);
            LoadLogsFromEngine(false);
        }

        // =========================
        //  SELL ALL (uses Market.SprzedajWszystko)
        // =========================
        private void SellAll_Click(object sender, RoutedEventArgs e)
        {
            var msg = _engine.Market.SprzedajWszystko(); 
            _engine.LoggerRepo.AddLog(msg);
            SystemSounds.Asterisk.Play();
            RenderAll(_engine.State);
            LoadLogsFromEngine(false);
        }

        // =========================
        //  INVENTORY (uses Rosliny.Clone + CompareTo)
        // =========================
        private void SortInventory_Click(object sender, RoutedEventArgs e)
        {
            // CompareTo z Rosliny: sort po PoziomNawodnienia
            _engine.State.Tomatoes.Sort();
            _engine.State.Apples.Sort();
            _engine.State.Roses.Sort();
            _engine.State.Cactile.Sort();

            _engine.LoggerRepo.AddLog("[GUI] Posortowano magazyn po nawodnieniu (CompareTo).");
            RenderAll(_engine.State);
            LoadLogsFromEngine(false);
        }

        private void CloneSelected_Click(object sender, RoutedEventArgs e)
        {
            if (InventoryList.SelectedItem is not InventoryItem inv) return;

            Rosliny? cloned = inv.Source.Clone() as Rosliny; 
            if (cloned == null) return;

            // klon dodajemy do odpowiedniej listy (bez dopisywania logiki silnika)
            if (cloned is Tomato t) _engine.State.Tomatoes.Add(t);
            else if (cloned is Apple a) _engine.State.Apples.Add(a);
            else if (cloned is Rose r) _engine.State.Roses.Add(r);
            else if (cloned is Cactus c) _engine.State.Cactile.Add(c);

            _engine.LoggerRepo.AddLog("[GUI] Utworzono klon sadzonki (Clone).");
            SystemSounds.Asterisk.Play();
            RenderAll(_engine.State);
            LoadLogsFromEngine(false);
        }

        private void PlantFromInventory_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedField == null)
            {
                _engine.LoggerRepo.AddLog("[GUI] Najpierw zaznacz pole na farmie.");
                return;
            }
            if (InventoryList.SelectedItem is not InventoryItem inv)
            {
                _engine.LoggerRepo.AddLog("[GUI] Wybierz roślinę z magazynu.");
                return;
            }

            var (row, col) = _selectedField.Value;
            if (_engine.IsOccupied(row, col))
            {
                _engine.LoggerRepo.AddLog($"[GUI] Pole ({row},{col}) zajęte.");
                return;
            }

            // Używamy istniejących właściwości IPositioned (Row/Col) – bez pisania metod w silniku
            inv.Source.Row = row;
            inv.Source.Col = col;

            _engine.LoggerRepo.AddLog($"[GUI] Posadzono z magazynu na ({row},{col}): {inv.Source.Nazwa}");
            SystemSounds.Asterisk.Play();
            RenderAll(_engine.State);
            LoadLogsFromEngine(false);
        }

        // =========================
        //  DEVICES (uses Market.KupZraszacz / KupPanelSloneczny)
        // =========================
        private void BuySprinkler_Click(object sender, RoutedEventArgs e)
        {
            var msg = _engine.Market.KupZraszacz(); 
            _engine.LoggerRepo.AddLog(msg);
            SystemSounds.Asterisk.Play();
            RenderAll(_engine.State);
            LoadLogsFromEngine(false);
        }

        private void BuySolar_Click(object sender, RoutedEventArgs e)
        {
            var msg = _engine.Market.KupPanelSloneczny(); 
            _engine.LoggerRepo.AddLog(msg);
            SystemSounds.Asterisk.Play();
            RenderAll(_engine.State);
            LoadLogsFromEngine(false);
        }

        private void ApplySensorThreshold_Click(object sender, RoutedEventArgs e)
        {
            if (!double.TryParse(SensorThresholdBox.Text.Replace('.', ','), out var thr))
                thr = 20.0;

            foreach (var s in _engine.State.Sensors)
                s.CriticalThreshold = thr;

            _engine.LoggerRepo.AddLog($"[GUI] Ustawiono Sensor.CriticalThreshold = {thr}");
            RenderAll(_engine.State);
            LoadLogsFromEngine(false);
        }

        // =========================
        //  FINANCE: Tax + Report + Transactions
        // =========================
        private void TaxCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_engine?.State?.Finance == null) return;

            var selected = (TaxCombo.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "NoTax";
            _engine.State.Finance.Tax = selected.StartsWith("FlatTax")
                ? new FlatTax(0.19m)
                : new NoTax();

            _engine.LoggerRepo.AddLog($"[GUI] Ustawiono Tax = {selected}");
            LoadLogsFromEngine(false);
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var from = DateTimeOffset.Parse(ReportFromBox.Text.Trim());
                var to = DateTimeOffset.Parse(ReportToBox.Text.Trim());

                var rep = _engine.State.Finance.GetReport(from, to, "Raport z GUI"); 
                ReportText.Text =
                    $"{rep.Title}\n" +
                    $"Revenue: {rep.Revenue}\n" +
                    $"Costs: {rep.Costs}\n" +
                    $"Profit: {rep.Profit}\n" +
                    $"Tax: {rep.Tax}\n" +
                    $"NetProfit: {rep.NetProfit}";

                _engine.LoggerRepo.AddLog("[GUI] Wygenerowano raport finansowy (GetReport).");
                LoadLogsFromEngine(false);
            }
            catch (Exception ex)
            {
                _engine.LoggerRepo.AddLog("[GUI] Błąd raportu: " + ex.Message);
                SystemSounds.Hand.Play();
            }
        }

        // =========================
        //  ENGINE TICK -> UI
        // =========================
        private void OnEngineTick(FarmState state)
        {
            Dispatcher.Invoke(() =>
            {
                UpdateStatsOnly(state);

                if (DateTime.Now - _lastGridRefresh >= GridRefreshInterval)
                {
                    _lastGridRefresh = DateTime.Now;
                    RenderGridFromEngine(state);
                    RefreshSelectionVisual();
                    RenderDevices(state);
                    RenderInventory(state);
                    RenderTransactions(state);
                }

                if (DateTime.Now - _lastLogRefresh >= LogRefreshInterval)
                {
                    _lastLogRefresh = DateTime.Now;
                    LoadLogsFromEngine(force: false);
                }
            });
        }

        private void RenderAll(FarmState state)
        {
            UpdateStatsOnly(state);
            RenderGridFromEngine(state);
            RefreshSelectionVisual();
            RenderDevices(state);
            RenderInventory(state);
            RenderTransactions(state);
        }

        private void UpdateStatsOnly(FarmState state)
        {
            TickText.Text = $"Tura: {state.CurrentTick}";
            BalanceText.Text = $"Stan konta: {state.Finance.Account.Balance}";

            SoilText.Text = $"SoilMoisture: {state.SoilMoisture:F1}";
            LightText.Text = $"LightLevel: {state.LightLevel:F1}";

            var allPlants = state.Tomatoes.Cast<Rosliny>()
                .Concat(state.Roses)
                .Concat(state.Cactile)
                .Concat(state.Apples)
                .ToList();

            double avgWater = allPlants.Count > 0 ? allPlants.Average(p => p.PoziomNawodnienia) : 0;
            double avgGrowth = allPlants.Count > 0 ? allPlants.Average(p => p.PoziomWzrostu) : 0;

            WaterBar.Value = Clamp0_100(avgWater);
            GrowthBar.Value = Clamp0_100(avgGrowth);
        }

        private void RenderGridFromEngine(FarmState state)
        {
            foreach (var btn in _cellButtons.Values)
                btn.Background = BrushFromHex("#252526");

            foreach (var t in state.Tomatoes.Where(p => p.Row >= 0 && p.Col >= 0))
                PaintCell(t.Row, t.Col, t.IsMature ? "#FFD600" : "#2E7D32");

            foreach (var r in state.Roses.Where(p => p.Row >= 0 && p.Col >= 0))
                PaintCell(r.Row, r.Col, r.IsMature ? "#FFD600" : "#AD1457");

            foreach (var c in state.Cactile.Where(p => p.Row >= 0 && p.Col >= 0))
                PaintCell(c.Row, c.Col, c.IsMature ? "#FFD600" : "#558B2F");

            foreach (var a in state.Apples.Where(p => p.Row >= 0 && p.Col >= 0))
                PaintCell(a.Row, a.Col, a.IsMature ? "#FFD600" : "#1565C0");
        }

        private void PaintCell(int row, int col, string hex)
        {
            if (_cellButtons.TryGetValue((row, col), out var btn))
                btn.Background = BrushFromHex(hex);
        }

        private void RenderInventory(FarmState state)
        {
            InventoryList.Items.Clear();

            var items = new List<InventoryItem>();
            void AddInv(IEnumerable<Rosliny> list)
            {
                foreach (var p in list.Where(x => x.Row < 0 && x.Col < 0))
                    items.Add(new InventoryItem(p));
            }

            AddInv(state.Tomatoes);
            AddInv(state.Apples);
            AddInv(state.Roses);
            AddInv(state.Cactile);

            foreach (var it in items)
                InventoryList.Items.Add(it);

            InventoryList.DisplayMemberPath = nameof(InventoryItem.View);
        }

        private void RenderDevices(FarmState state)
        {
            SprinklersList.Items.Clear();
            foreach (var s in state.Sprinklers)
                SprinklersList.Items.Add($"{s.Name} | IsOn={s.IsOn} | Cena={s.Cena}");

            SolarsList.Items.Clear();
            foreach (var s in state.Solars)
                SolarsList.Items.Add($"{s.Name} | IsOn={s.IsOn} | Cena={s.Cena}");
        }

        private void RenderTransactions(FarmState state)
        {
            TransactionsGrid.ItemsSource = state.Finance.Transactions
                .Select(t => new
                {
                    OccurredAt = t.OccurredAt.ToString("u"),
                    Type = t.Type.ToString(),
                    Category = t.Category.ToString(),
                    Amount = t.Amount.ToString(),
                    Description = t.Description
                })
                .ToList();
        }

        private void LoadLogsFromEngine(bool force)
        {
            var logs = _engine.LoggerRepo.GetLogs(400);

            if (!force && logs.Count <= _lastLogCount)
                return;

            // DŹWIĘK: alarmy sensora / finanse
            if (_lastLogCount >= 0 && logs.Count > _lastLogCount)
            {
                var newLines = logs.Skip(_lastLogCount).ToList();
                if (newLines.Any(l => l.Contains("[agro.Core] Sensor:")))
                    SystemSounds.Exclamation.Play();
                if (newLines.Any(l => l.Contains("[FINANSE]")))
                    SystemSounds.Beep.Play();
            }

            _lastLogCount = logs.Count;

            LogList.Items.Clear();
            foreach (var line in logs)
                LogList.Items.Add(line);

            if (LogList.Items.Count > 0)
                LogList.ScrollIntoView(LogList.Items[^1]);
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            _engine?.StopSimulation();
        }

        private static SolidColorBrush BrushFromHex(string hex)
            => (SolidColorBrush)new BrushConverter().ConvertFromString(hex);

        private static double Clamp0_100(double v)
        {
            if (v < 0) return 0;
            if (v > 100) return 100;
            return v;
        }

        private sealed class InventoryItem
        {
            public Rosliny Source { get; }
            public string View => $"{Source.Nazwa} | Wzrost={Source.PoziomWzrostu:0}% | Woda={Source.PoziomNawodnienia:0}% | UV={Source.PoziomNaslonecznienia:0}%";

            public InventoryItem(Rosliny src) => Source = src;
        }
    }
}
