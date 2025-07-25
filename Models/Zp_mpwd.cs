using System.ComponentModel.DataAnnotations;

namespace Models
{
    [Lib.Attributes.Table("zp_mpwd")]
    public class Zp_mpwd
    {
        [Key]
        public string pwd_user { get; set; }

        public string pwd_pass { get; set; }

        public string pwd_rmk1 { get; set; }

        public string pwd_data_1 { get; set; }

    }
}

