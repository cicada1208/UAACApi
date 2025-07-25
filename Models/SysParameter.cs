using System;
using System.ComponentModel.DataAnnotations;

namespace Models
{
    [Lib.Attributes.Table("ni_SysParameter")]
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
