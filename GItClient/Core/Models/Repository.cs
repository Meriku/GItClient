using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GItClient.Core.Models
{
    public class Repository
    {
        // TODO: just MVP

        public string Name { get; set; }
        public string Link { get; set; }
        public int CommitsCount { get; set; }

        public Repository() { }

    }
}
