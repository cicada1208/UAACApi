namespace Models
{
    public class RoleUserPermissionParam
    {
        public RoleUserPermissionParamOption? Option { get; set; }

        public string UserId { get; set; }

        public string SysId { get; set; }

        public string RoleId { get; set; }

        public bool? Activate { get; set; }
    }

    public enum RoleUserPermissionParamOption
    {
        /// <summary>
        /// 依使用者查詢
        /// </summary>
        ByUserInfo = 1,
        /// <summary>
        /// 依系統代碼查詢
        /// </summary>
        BySysId = 2,
        /// <summary>
        /// 依角色群組查詢
        /// </summary>
        ByRoleId = 3
    }
}
