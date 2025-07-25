using Lib;
using Models;
using Params;
using Repositorys.NISDB;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Repositorys.UAAC
{
    public class RoleRepository : UAACBaseRepository<Role>
    {
        public override async Task<ApiResult<Role>> Insert(Role param)
        {
            var result = await Get(new Role
            {
                RoleId = param.RoleId
            });

            if (result.Data.Any())
                return new ApiResult<Role>(false, msg: MsgParam.KeyDuplication);

            if (param.MDateTime.IsNullOrWhiteSpace())
                param.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            if (param.RoleId.IsNullOrWhiteSpace())
                param.RoleId = Guid.NewGuid().ToString();
            return await base.Insert(param);
        }

        public override Task<ApiResult<Role>> Update(Role param)
        {
            param.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return base.Update(param);
        }

    }
}
