using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LegsandRegsCS.Data;
using System.ComponentModel.DataAnnotations;

namespace LegsandRegsCS.Models
{
    public class Act : ActSummary
    {
        public List<ActReg> regs { get; set; }
    }
}
