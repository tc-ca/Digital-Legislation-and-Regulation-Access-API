using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LegsandRegsCS.Models
{
    public class ActId
    {
        //This class only exists for accepting the compound Act ID through HTTP Post calls. It is not used to store Act IDs in the database.
        public string uniqueId { get; set; }
        public string lang { get; set; }
    }
}
