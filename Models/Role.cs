using System.ComponentModel.DataAnnotations;

namespace Models
{
    [Lib.Attributes.Table("Role")]
    public class Role
    {
        [Key]
        public string RoleId { get; set; }

        public string RoleName { get; set; }

        public string Description { get; set; }

        public bool? Activate { get; set; }

        public string MUserId { get; set; }

        public string MDateTime { get; set; }

    }
}
