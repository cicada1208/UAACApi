using System.ComponentModel.DataAnnotations;

namespace Models
{
    [Lib.Attributes.Table("WORKERS")]
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
