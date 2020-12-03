using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LegsandRegsCS.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegsandRegsCS.Models
{
    [Table("RY001_ACT")]    //WARNING: If this is ever changed, the custom SQL statement in the SeedData class must alse be updated
                            //Due to technical limitations with EF Core, the table name cannot be populated from a common variable
    public class Act : ActSummary
    {        
        public List<ActReg> regs { get; set; }
    }
}
