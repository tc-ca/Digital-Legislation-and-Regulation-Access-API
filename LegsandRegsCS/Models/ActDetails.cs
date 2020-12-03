using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegsandRegsCS.Models
{
    [Table("RY004_ACT_DETAILS")]    //WARNING: If this is ever changed, the custom SQL statement in the SeedData class must alse be updated
                                    //Due to technical limitations with EF Core, the table name cannot be populated from a common variable
    public class ActDetails
    {
        [Column("ACT_ID")]
        public string uniqueId { get; set; }
        [Column("LANG_CD")]
        public string lang { get; set; }
        [Column("ACT_FULL_DETAILS_TXT")]
        public string fullDetails { get; set; }
    }
}
