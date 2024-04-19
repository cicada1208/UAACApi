using Params;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Lib.Attributes.Table(DBParam.DBType.SYBASE, DBParam.DBName.NIS, "ni_SysParameter")]
    public class SysParameter 
    {
        [Key]
        public string sysParameterId { get; set; }

        public string parameterName { get; set; }

        public string value { get; set; }

        public string description { get; set; }

        public bool? isActive { get; set; }

        public string systemUserId { get; set; }

        public DateTime? systemDt { get; set; }

    }
}
