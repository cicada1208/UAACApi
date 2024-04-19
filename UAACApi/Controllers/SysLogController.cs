using Lib;
using Lib.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Repositorys;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UAACApi.Controllers
{
    public class SysLogController : BaseController
    {
        public SysLogController(IOptionsMonitor<AppSettings> settings,
            DBContext db,
            ApiUtilLocator apiUtils,
            UtilLocator utils)
            : base(settings.CurrentValue, db, apiUtils, utils) { }

        [HttpGet]
        public Task<ApiResult<List<SysLog>>> Get([FromQuery] SysLog param) =>
            DB.SysLogRepository.Get(param);

        [HttpPost]
        public Task<ApiResult<SysLog>> Insert(SysLog param)
        {
            if (param.UserIP.IsNullOrWhiteSpace())
                param.UserIP = RemoteIpAddress;
            return DB.SysLogRepository.Insert(param);
        }

    }
}
