using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LegsandRegsCS.Special_Data_Types
{
    public class Act
    {
        string uniqueId { get; set; }
        string officialNum { get; set; }
        string title { get; set; }
        string lang { get; set; }
        string currentToDate { get; set; }

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
