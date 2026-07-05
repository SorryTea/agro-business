using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace _01_agro.Core
{
    public enum PlantType
    {
        Vegetable,
        Fruit,
        Flower,
        Succulent
    }

    /// <summary>
    /// Base plant: each tick its moisture, sunlight and growth update, with the growth
    /// rate defined by each concrete plant type.
    /// </summary>

    public abstract class Plant : ITickable, ICloneable, IComparable<Plant>, IPositioned
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]


        public string Name { get; set; }
        public PlantType Type { get; set; }

        public int Row { get; set; } = -1;
        public int Col { get; set; } = -1;

        private float _price;
        public float Price
        {
            get => _price;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(Price), "Cena nie może być ujemna.");
                }

                _price = value;
            }
        }

        public float SalePrice { get; set; }


        public float GrowthLevel { get; set; } = 0;
        public float MoistureLevel { get; set; } = 20;
        public float SunlightLevel { get; set; } = 30;

        public bool IsDead { get; set; } = false;

        [NotMapped]
        public bool IsMature => GrowthLevel >= 100;

        protected Plant(string name, PlantType type)
        {
            Name = name;
            Type = type;
        }

        protected Plant() { }

        public virtual void Tick(FarmState state)
        {
            if (IsDead || IsMature)
            {
                return;
            }

            // Water: consumes the shared soil moisture.
            if (state.SoilMoisture >= 5)
            {
                state.SoilMoisture -= 5;
                MoistureLevel += 10;
                if (MoistureLevel > 100)
                {
                    MoistureLevel = 100;
                }
            }
            else
            {
                MoistureLevel -= 5;
            }

            // Sunlight: we don't consume the global resource, only draw from it.
            if (state.LightLevel >= 10)
            {
                SunlightLevel += 10;
                if (SunlightLevel > 100)
                {
                    SunlightLevel = 100;
                }
            }
            else
            {
                SunlightLevel -= 10;
            }

            if (MoistureLevel <= 0 || SunlightLevel <= 0)
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
            Name = "Uschnięty " + Name;
        }

        protected abstract void DoSpecificGrowth();

        public object Clone()
        {
            var clone = (Plant)MemberwiseClone();
            clone.Id = Guid.NewGuid();
            clone.Name = $"{Name} (Szczepka)";
            clone.GrowthLevel = 0; // A clone is a fresh cutting, so growth starts at 0.
            return clone;
        }

        public int CompareTo(Plant? other)
        {
            if (other == null)
            {
                return 1;
            }

            return MoistureLevel.CompareTo(other.MoistureLevel);
        }
    }
}
