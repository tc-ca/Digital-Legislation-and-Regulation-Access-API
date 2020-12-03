using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegsandRegsCS.Models
{
    [Table("RY005_REG_DETAILS")]    //WARNING: If this is ever changed, the custom SQL statement in the SeedData class must alse be updated
                                    //Due to technical limitations with EF Core, the table name cannot be populated from a common variable
    public class RegDetails
    {
        [Column("REG_ID")]
        public string id { get; set; }
        [Column("REG_FULL_DETAILS_TXT")]
        public string fullDetails { get; set; }
    }
}
