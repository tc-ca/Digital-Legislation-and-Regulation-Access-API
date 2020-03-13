using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LegsandRegsCS.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LegsandRegsCS.Models
{
    public class Reg
    {
        public string id { get; set; }
        public string otherLangId { get; set; }
        public string uniqueId { get; set; }
        public string title { get; set; }
        public string lang { get; set; }
        public string currentToDate { get; set; }
        public string details { get; set; }
    }
}
