using System;
using System.Collections.Generic;
using System.Linq;

namespace _02_agro.Data
{
    /// <summary>
    /// In-memory store for log and notification messages.
    /// </summary>
    public class LogRepo
    {
        private readonly List<string> _logs = new();

        public void AddLog(string message)
        {
            _logs.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
        }

        public List<string> GetLogs(int takeLast = 200)
        {
            if (takeLast <= 0)
            {
                return new List<string>();
            }

            return _logs
                .Skip(Math.Max(0, _logs.Count - takeLast))
                .ToList();
        }
    }
}
