using Params;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Lib.Attributes.Table(DBParam.DBType.SYBASE, DBParam.DBName.SYB2, "zp_mpwd")]
    public class Zp_mpwd
    {
        [Key]
        public string pwd_user { get; set; }

        public string pwd_pass { get; set; }

        public string pwd_rmk1 { get; set; }

        public string pwd_data_1 { get; set; }

    }
}

