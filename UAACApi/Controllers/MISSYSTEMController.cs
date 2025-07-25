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
    public class MISSYSTEMController : BaseController
    {
        public MISSYSTEMController(IOptionsMonitor<AppSettings> settings,
            DBContext db,
            ApiUtilLocator apiUtils,
            UtilLocator utils)
            : base(settings.CurrentValue, db, apiUtils, utils) { }

        [HttpGet]
        public Task<ApiResult<List<MISSYSTEM>>> Get([FromQuery] MISSYSTEM param) =>
            DB.MISSYSTEMRepository.Get(param);

        [HttpGet("[action]")]
        public Task<ApiResult<List<USERMISSYSTEM>>> GetUSERMISSYSTEM(string EMPNO) =>
            DB.MISSYSTEMRepository.GetUSERMISSYSTEM(EMPNO);

    }
}
