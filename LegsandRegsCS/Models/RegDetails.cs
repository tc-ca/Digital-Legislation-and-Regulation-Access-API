﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace LegsandRegsCS.Models
{
    [Table("RY005_REG_DETAILS")]
    public class RegDetails
    {
        [Column("REG_ID")]
        public string id { get; set; }
        [Column("REG_FULL_DETAILS_TXT")]
        public string fullDetails { get; set; }
    }
}
