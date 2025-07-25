using Lib;
using Models;
using Repositorys.NISDB;
using System;
using System.Threading.Tasks;

namespace Repositorys.UAAC
{
    public class SysLogRepository : UAACBaseRepository<SysLog>
    {
        public override Task<ApiResult<SysLog>> Insert(SysLog param)
        {
            if (param.LogDateTime.IsNullOrWhiteSpace())
                param.LogDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            return base.Insert(param);
        }
    }
}
