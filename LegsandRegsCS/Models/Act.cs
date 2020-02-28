using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LegsandRegsCS.Data;
using System.ComponentModel.DataAnnotations;

namespace LegsandRegsCS.Models
{
    public class Act
    {
        public string uniqueId { get; set; }
        public string officialNum { get; set; }
        public string title { get; set; }
        public string lang { get; set; }
        public string currentToDate { get; set; }
        //public List<string> regsUnderAct { get; set; }
    }
}
