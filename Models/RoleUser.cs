using System.ComponentModel.DataAnnotations;

namespace Models
{
    [Lib.Attributes.Table("RoleUser")]
    public class RoleUser
    {
        [Key]
        public string RoleUserId { get; set; }

        public string RoleId { get; set; }

        public string CpnyId { get; set; }

        public string DeptNo { get; set; }

        public string Possie { get; set; }

        public string Attribute { get; set; }

        public string UserId { get; set; }

        public bool? Activate { get; set; }

        public string MUserId { get; set; }

        public string MDateTime { get; set; }

    }
}
