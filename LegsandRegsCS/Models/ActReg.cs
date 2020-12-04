using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegsandRegsCS.Models
{
    [Table("TY003_XREF_RY001_RY002")]
    public class ActReg
    {
        [Column("ACT_ID")]
        public string actUniqueId { get; set; }
        [Column("ACT_LANG_CD")]
        public string actLang { get; set; }
        [Column("REG_ID")]
        public string regId { get; set; }
        public Reg reg { get; set; }
    }
}
