using System.ComponentModel.DataAnnotations;

namespace Models
{
    [Lib.Attributes.Table("RolePermission")]
    public class RolePermission
    {
        [Key]
        public string RolePermissionId { get; set; }

        public string RoleId { get; set; }

        public string SysId { get; set; }

        public string FuncId { get; set; }

        public bool? QueryAuth { get; set; }

        public bool? AddAuth { get; set; }

        public bool? ModifyAuth { get; set; }

        public bool? DeleteAuth { get; set; }

        public bool? ExportAuth { get; set; }

        public bool? PrintAuth { get; set; }

        public bool? Activate { get; set; }

        public string MUserId { get; set; }

        public string MDateTime { get; set; }

    }
}
