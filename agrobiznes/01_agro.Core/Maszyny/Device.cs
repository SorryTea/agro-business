using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _01_agro.Core
{
    /// <summary>
    /// Klasa abstrakcyjna Urządzenie, która poprzez interfejs ITickable będzie odświeżana
    /// </summary>
    public abstract class Device : ITickable
    {
        [Key] // Klucz główny w bazie
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // Używamy własnego GUID, nie auto-number
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public bool IsOn { get; set; }

        public float Cena { get; set; }
        public abstract void Tick(FarmState state);
        
    }
}
