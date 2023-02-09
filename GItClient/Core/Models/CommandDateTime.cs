using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core.Models
{
    internal class HistoryElement
    {
        public string Value { get; set; }
        public DateTime DateTime { get; set; }
        public HistoryType Type { get; set; }

        internal HistoryElement(string value, HistoryType _type, DateTime? time = null)
        {
            Value = value;
            DateTime = time ?? DateTime.Now;
            Type = _type;
        }
    }

    enum HistoryType
    {
        Request,
        Response
    }
}
