using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegsandRegsCS.Models
{
    public class ActSummary
    {
        [Column("ACT_ID")]
        public string uniqueId { get; set; }
        [Column("LANG_CD")]
        public string lang { get; set; }
        [Column("OFFICIAL_NUM_LBL")]
        public string officialNum { get; set; }
        [Column("ACT_TITLE_TXT")]
        public string title { get; set; }
        [Column("VALID_TO_DTE")]
        public DateTime currentToDate { get; set; }
    }
}
