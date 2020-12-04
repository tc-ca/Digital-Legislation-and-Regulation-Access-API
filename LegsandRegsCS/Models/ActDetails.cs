using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegsandRegsCS.Models
{
    [Table("RY004_ACT_DETAIL")]
    public class ActDetails
    {
        [Column("ACT_ID")]
        public string uniqueId { get; set; }
        [Column("LANG_CD")]
        public string lang { get; set; }
        [Column("ACT_FULL_DETAIL_TXT")]
        public string fullDetails { get; set; }
    }
}
