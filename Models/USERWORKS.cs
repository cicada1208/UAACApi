using System.ComponentModel.DataAnnotations;

namespace Models
{
    [Lib.Attributes.Table("USERWORKS")]
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

