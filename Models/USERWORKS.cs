using Params;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Lib.Attributes.Table(DBParam.DBType.SQLSERVER, DBParam.DBName.MISSYS, "USERWORKS")]
    public class USERWORKS
    {
        [Key]
        public string EMPNO { get; set; }

        [Key]
        public string SYSID { get; set; }

        public string ROLE { get; set; }

        public string USECOUNT { get; set; }

    }
}

