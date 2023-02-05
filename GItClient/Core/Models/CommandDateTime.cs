using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core.Models
{
    internal class CommandDateTime
    {
        public string Command { get; set; }
        public DateTime DateTime { get; set; }

        internal CommandDateTime(string command, DateTime? time = null)
        {
            Command = command;
            DateTime = time ?? DateTime.Now;
        }
    }
}
