using System;
using System.Collections.Generic;
using System.Linq;
using _01_agro.Core;
using _02_agro.Data;
using _01_agro.Core.Economy;


namespace _03_agro.Logic
{
    /// <summary>
    /// Klasa ta odpowiada za zmiany na rynku, transakcje: kupno, sprzedaż
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

        private bool TryPay(float koszt, TransactionCategory category, string description, out string message)
        {
            var kosztMoney = new Money((decimal)koszt, "PLN");

            try
            {
                _state.Finance.Apply(new PurchaseTransaction(
                    kosztMoney,
                    category,
                    description
                ));

                message = string.Empty;
                return true;
            }
            catch (InvalidOperationException)
            {
                message = $"BŁĄD: Brak środków. Koszt: {kosztMoney}, saldo: {_state.Finance.Account.Balance}";
                _logger.AddLog(message);
                return false;
            }
        }
        public bool TryBuyPlant(float cost, string plantName, out string message)
        {
            return TryPay(cost, TransactionCategory.Other, $"Zakup rośliny: {plantName}", out message);
        }


        // ==========================================
        // 1. KUPOWANIE (PROSTE)
        // ==========================================


        public string KupPomidory(int ilosc)
        {
            // Tworzymy "prototyp", żeby sprawdzić aktualną cenę w tym sezonie
            var wzorzec = new Tomato();
            float koszt = wzorzec.Cena * ilosc; 

            // [MIEJSCE NA IF FINANSOWY] 
           if (!TryPay(koszt, TransactionCategory.Seeds, $"Zakup: Pomidory x{ilosc}", out var err))
            {
                return err;
            }


            for (int i = 0; i < ilosc; i++)
            {
                // Tworzymy nową sztukę
                var t = new Tomato();
                _state.Tomatoes.Add(t);
            }

            string msg = $"SKLEP: Kupiono {ilosc} pomidorów. Koszt: {koszt:C}";
            _logger.AddLog(msg);
            return msg;
        }

        public string KupJablka(int ilosc)
        {
            var wzorzec = new Apple();
            float koszt = wzorzec.Cena * ilosc;

            // [IF FINANSOWY] 
            if (!TryPay(koszt, TransactionCategory.Seeds, $"Zakup: Jabłka x{ilosc}", out var err))
            {
                return err;
            }

            for (int i = 0; i < ilosc; i++)
            {
                // Tworzymy nową sztukę
                var t = new Apple();

                _state.Apples.Add(t);
            }
            string msg = $"SKLEP: Kupiono {ilosc} jabłek. Koszt: {koszt:C}";
            _logger.AddLog(msg);
            return msg;
        }

        public string KupKaktusy(int ilosc)
        {
            var wzorzec = new Cactus();
            float koszt = wzorzec.Cena * ilosc;

            // [IF FINANSOWY] 
            if (!TryPay(koszt, TransactionCategory.Seeds, $"Zakup: Kaktusy x{ilosc}", out var err))
            {
                return err;
            }

            for (int i = 0; i < ilosc; i++)
            {
                // Tworzymy nową sztukę
                var t = new Cactus();

                _state.Cactile.Add(t);
            }
            string msg = $"SKLEP: Kupiono {ilosc} kaktusów. Koszt: {koszt:C}";
            _logger.AddLog(msg);
            return msg;
        }

        public string KupRóże(int ilosc)
        {
            var wzorzec = new Rose();
            float koszt = wzorzec.Cena * ilosc;

            // [IF FINANSOWY] 
            if (!TryPay(koszt, TransactionCategory.Seeds, $"Zakup: Róży x{ilosc}", out var err))
            {
                return err;
            }

            for (int i = 0; i < ilosc; i++)
            {
                // Tworzymy nową sztukę
                var t = new Rose();

                _state.Roses.Add(t);
            }
            string msg = $"SKLEP: Kupiono {ilosc} róż. Koszt: {koszt:C}";
            _logger.AddLog(msg);
            return msg;
        }

        // ==========================================
        // 2. SPRZEDAŻ (BARDZO PROSTA)
        // ==========================================

        public string SprzedajWszystko()
        {
            float zarobekCalkowity = 0;
            int iloscCalkowita = 0;

            

            var wynikPomidory = SprzedajZListy(_state.Tomatoes);
            zarobekCalkowity += wynikPomidory.zarobek;
            iloscCalkowita += wynikPomidory.ilosc;

            var wynikJablka = SprzedajZListy(_state.Apples);
            zarobekCalkowity += wynikJablka.zarobek; 
            iloscCalkowita += wynikJablka.ilosc;

            var wynikKaktusy = SprzedajZListy(_state.Cactile);
            zarobekCalkowity += wynikKaktusy.zarobek;
            iloscCalkowita += wynikKaktusy.ilosc;

            var wynikRoze = SprzedajZListy(_state.Roses);
            zarobekCalkowity += wynikRoze.zarobek;
            iloscCalkowita += wynikRoze.ilosc;

            if (iloscCalkowita > 0)
            {
                // --- WPŁATA NA KONTO ---
                var revenueMoney = new Money((decimal)zarobekCalkowity, "PLN");
                _state.Finance.Apply(new SaleTransaction(
                    revenueMoney,
                    TransactionCategory.Sales,
                    $"Sprzedaż roślin: {iloscCalkowita} szt."
                ));

                string msg = $"SKUP: Sprzedano {iloscCalkowita} roślin za {zarobekCalkowity:C}.";
                _logger.AddLog(msg);
                return msg;
            }

            return "SKUP: Magazyn pusty (brak dojrzałych roślin).";
        }
        public bool TrySellAt(int row, int col, out string message)
        {
            // 1) Szukamy rośliny na pozycji (row,col) w konkretnych listach
            Tomato tomato = _state.Tomatoes.FirstOrDefault(p => p.Row == row && p.Col == col);
            if (tomato != null)
                return SellSpecific(_state.Tomatoes, tomato, row, col, out message);

            Apple apple = _state.Apples.FirstOrDefault(p => p.Row == row && p.Col == col);
            if (apple != null)
                return SellSpecific(_state.Apples, apple, row, col, out message);

            Cactus cactus = _state.Cactile.FirstOrDefault(p => p.Row == row && p.Col == col);
            if (cactus != null)
                return SellSpecific(_state.Cactile, cactus, row, col, out message);

            Rose rose = _state.Roses.FirstOrDefault(p => p.Row == row && p.Col == col);
            if (rose != null)
                return SellSpecific(_state.Roses, rose, row, col, out message);

            message = $"SKUP: Pole ({row},{col}) jest puste.";
            return false;
        }

        private bool SellSpecific<T>(List<T> list, T plant, int row, int col, out string message)
            where T : Rosliny
        {
            // 2) (Opcjonalnie) sprzedajemy tylko dojrzałe i nie martwe
            if (!plant.IsMature || plant.IsDead)
            {
                message = $"SKUP: Roślina na ({row},{col}) nie jest gotowa do sprzedaży.";
                return false;
            }

            // 3) Zarobek
            float income = plant.CenaSprzedazy;

            // 4) Usuwamy z listy
            list.Remove(plant);

            // 5) Wpłata na konto
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



        // ==========================================
        // 3. SILNIK SPRZEDAŻY (GENERYCZNY)
        // ==========================================

        // Ta metoda przyjmuje dowolną listę roślin (T : Roslina)
        private (int ilosc, float zarobek) SprzedajZListy<T>(List<T> listaRoslin) where T : Rosliny
        {
            // 1. Wybierz te do sprzedania
            var doSprzedania = listaRoslin.Where(r => r.IsMature && !r.IsDead).ToList();

            if (doSprzedania.Count == 0) return (0, 0);

            // 2. Policz zysk (Suma cen sprzedaży konkretnych obiektów)
            // Dzięki temu, że cena jest w roślinie, to działa automatycznie!
            float zysk = doSprzedania.Sum(r => r.CenaSprzedazy);

            // 3. Usuń fizycznie z farmy
            listaRoslin.RemoveAll(r => r.IsMature && !r.IsDead);

            return (doSprzedania.Count, zysk);
        }

        // ==========================================
        // 4. KUPOWANIE MASZYN
        // ==========================================

        public string KupZraszacz()
        {
            var nowaMaszyna = new Sprinkler();
            return KupMaszyneKonkretna(nowaMaszyna, _state.Sprinklers);
        }

        public string KupPanelSloneczny()
        {
            var nowaMaszyna = new Solar();
            return KupMaszyneKonkretna(nowaMaszyna, _state.Solars);
        }

        // --- POMOCNICZA METODA DLA MASZYN ---
        
        private string KupMaszyneKonkretna<T>(T maszyna, List<T> listaDocelowa) where T : Device
        {
            float koszt = maszyna.Cena;


            // 1. Walidacja finansowa (Zostawiam miejsce) + 2. Pobranie pieniędzy
            if (!TryPay(koszt, TransactionCategory.Other, $"Zakup maszyny: {maszyna.Name}", out var err))
            {
                return err;
            }


            // 3. Dodanie do farmy
            listaDocelowa.Add(maszyna);

            string msg = $"SKLEP: Zakupiono maszynę: {maszyna.Name}. Koszt: {koszt:C}";
            _logger.AddLog(msg);
            return msg;
        }
    }
}