using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LegsandRegsCS.Data;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegsandRegsCS.Models
{
    [Table("RY002_REG")]    //WARNING: If this is ever changed, the custom SQL statement in the SeedData class must alse be updated
                            //Due to technical limitations with EF Core, the table name cannot be populated from a common variable
    public class Reg
    {
        [Column("REG_ID")]
        public string id { get; set; }
        [Column("OTHER_LANG_REG_ID")]
        public string otherLangId { get; set; }
        [Column("PARENT_ACT_ID")]
        public string uniqueId { get; set; }
        [Column("LANG_CD")]
        public string title { get; set; }
        [Column("REG_TITLE_TXT")]
        public string lang { get; set; }
        [Column("VALID_TO_DTE")]
        public DateTime currentToDate { get; set; }
    }
}
