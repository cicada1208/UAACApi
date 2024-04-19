using Params;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Lib.Attributes.Table(DBParam.DBType.SQLSERVER, DBParam.DBName.MISSYS, "WORKERS")]
    public class WORKERS
    {
        [Key]
        public string EMPNO { get; set; }

        public string NAME { get; set; }

        public string SEX { get; set; }

        public string PASSWORD { get; set; }

        public string WORKTYPE { get; set; }
    }
}
