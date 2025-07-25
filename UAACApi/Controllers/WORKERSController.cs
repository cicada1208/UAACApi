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
    public class WORKERSController : BaseController
    {
        public WORKERSController(IOptionsMonitor<AppSettings> settings,
            DBContext db,
            ApiUtilLocator apiUtils,
            UtilLocator utils)
            : base(settings.CurrentValue, db, apiUtils, utils) { }

        [HttpGet]
        public Task<ApiResult<List<WORKERS>>> Get([FromQuery] WORKERS param) =>
            DB.WORKERSRepository.Get(param);

        [HttpGet("[action]")]
        public Task<ApiResult<WORKERS>> GetWORKER(string EMPNO) =>
            DB.WORKERSRepository.GetWORKER(EMPNO);

    }
}
