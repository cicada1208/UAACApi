namespace Models
{
    public class SysFuncPermission
    {
        /// <summary>
        /// RolePermission or UserPermission
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// RolePermissionId or UserPermissionId
        /// </summary>
        public string Id { get; set; }

        public string RoleId { get; set; }

        public string UserId { get; set; }

        /// <summary>
        /// 系統Id
        /// </summary>
        public string SysId { get; set; }

        /// <summary>
        /// 功能Id
        /// </summary>
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
