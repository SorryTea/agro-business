using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.ComponentModel.DataAnnotations;


namespace _02_agro.Data
{
    /// <summary>
    /// Klasa WpisWSystemie tworzy szablon z atrybutów, który będzie zapisywany jako wpis
    /// </summary>
    public class SystemLog
    {
        [Key]
        public int LogId { get; set; }
        public string Message { get; set; }
        public DateTime Date { get; set; }
    }
}
