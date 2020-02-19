using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LegsandRegsCS.Special_Data_Types
{
    public class Act
    {
        public string uniqueId { get; set; }
        public string officialNum { get; set; }
        public string title { get; set; }
        public string lang { get; set; }
        public string currentToDate { get; set; }

        public Act(string uniqueId, string officialNum, string title, string lang, string currentToDate)
        {
            this.uniqueId = uniqueId;
            this.officialNum = officialNum;
            this.title = title;
            this.lang = lang;
            this.currentToDate = currentToDate;
        }
    }
}
