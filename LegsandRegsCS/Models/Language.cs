using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegsandRegsCS.Models
{
    [Table("TC010_LANGUAGE")]
    public class Language
    {
        [Column("LANG_CD")]
        public string langCode { get; set; }
        [Column("LANG_ELBL")]
        public string englishLabel { get; set; }
        [Column("LANG_FLBL")]
        public string frenchLabel { get; set; }
    }
}
