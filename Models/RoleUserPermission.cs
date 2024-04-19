using System.Collections.Generic;

namespace Models
{
    public class RoleUserPermission
    {
        public List<RoleUser> RoleUsers { get; set; }

        public List<RolePermission> RolePermissions { get; set; }
    }
}
