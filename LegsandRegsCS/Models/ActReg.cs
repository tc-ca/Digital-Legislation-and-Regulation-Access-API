using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LegsandRegsCS.Models
{
    public class ActReg
    {
        public Act act { get; set; }
        public string actUniqueId { get; set; }
        public string actLang { get; set; }
        public Reg reg { get; set; }
        public string regId { get; set; }
    }
}
