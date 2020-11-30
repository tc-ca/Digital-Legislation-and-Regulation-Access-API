using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LegsandRegsCS.Models
{
    public class ActSummary
    {
        public string uniqueId { get; set; }
        public string officialNum { get; set; }
        public string title { get; set; }
        public string lang { get; set; }
        public DateTime currentToDate { get; set; }
    }
}
