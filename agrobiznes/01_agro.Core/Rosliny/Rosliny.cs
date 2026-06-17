using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _01_agro.Core
{
    public enum TypRosliny
    {
        Warzywo,
        Owoc,
        Kwiat,
        Sukulent
    }

    /// <summary>
    /// Abstrakcyjna klasa Rośliny, która poprzez interfejsy jest odświeżana, klonowalna i porównywalna z inną rośliną
    /// jej atrybuty zmieniaja się z każdym odświeżeniem w zależności od rodzaju rośliny (naslonecznienie, nawodnienie, wzrost)
    /// </summary>

    public abstract class Rosliny : ITickable, ICloneable, IComparable<Rosliny>, IPositioned
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]


        public string Nazwa { get; set; }
        public TypRosliny Typ { get; set; }

        public int Row { get; set; } = -1;
        public int Col { get; set; } = -1;

        private float _cena;
        public float Cena
        {
            get => _cena;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Cena), "Cena nie może być ujemna.");
                }

                _cena = value;
            }
        }

        public float CenaSprzedazy { get; set; }


        public float PoziomWzrostu { get; set; } = 0;
        public float PoziomNawodnienia { get; set; } = 20;
        public float PoziomNaslonecznienia { get; set; } = 30;

        public bool IsDead { get; set; } = false;

        [NotMapped]
        public bool IsMature => PoziomWzrostu >= 100;

        // --- KONSTRUKTORY ---


        protected Rosliny(string nazwa, TypRosliny typ)
        {
            Nazwa = nazwa;
            Typ = typ;
        }

        // Konstruktor dla Entity Framework (musi być pusty)
        protected Rosliny() { }

        // --- LOGIKA ---

        public virtual void Tick(FarmState state)
        {
            if (IsDead || IsMature)
            {
                return;
            }

            // 1. WODA (Zużywamy zasoby gleby)
            if (state.SoilMoisture >= 5)
            {
                state.SoilMoisture -= 5;
                PoziomNawodnienia += 10;
                if (PoziomNawodnienia > 100)
                {
                    PoziomNawodnienia = 100;
                }
            }
            else
            {
                PoziomNawodnienia -= 5;
            }

            // 2. SŁOŃCE (Nie zużywamy zasobów globalnych, tylko z nich korzystamy!)
            // POPRAWKA: Usunąłem state.LightLevel -= 10;
            if (state.LightLevel >= 10)
            {
                PoziomNaslonecznienia += 10;
                if (PoziomNaslonecznienia > 100)
                {
                    PoziomNaslonecznienia = 100;
                }
            }
            else
            {
                PoziomNaslonecznienia -= 10;
            }

            // 3. ŻYCIE I ŚMIERĆ
            if (PoziomNawodnienia <= 0 || PoziomNaslonecznienia <= 0)
            {
                Die(state);
            }
            else
            {
                DoSpecificGrowth();
            }
        }

        protected void Die(FarmState state)
        {
            IsDead = true;
            Nazwa = "Uschnięty " + Nazwa; // Opcjonalnie: zmiana nazwy
        }

        protected abstract void DoSpecificGrowth();

        // --- INTERFEJSY ---

        public object Clone()
        {
            var clone = (Rosliny)MemberwiseClone();
            clone.Id = Guid.NewGuid();
            clone.Nazwa = $"{Nazwa} (Szczepka)";
            clone.PoziomWzrostu = 0; // Resetujemy wzrost dla nowej sadzonki
            return clone;
        }

        public int CompareTo(Rosliny? other)
        {
            if (other == null)
            {
                return 1;
            }

            return PoziomNawodnienia.CompareTo(other.PoziomNawodnienia);
        }
    }
}
