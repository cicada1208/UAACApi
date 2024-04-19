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
    public class Mg_mnidController : BaseController
    {
        public Mg_mnidController(IOptionsMonitor<AppSettings> settings,
            DBContext db,
            ApiUtilLocator apiUtils,
            UtilLocator utils)
            : base(settings.CurrentValue, db, apiUtils, utils) { }

        [HttpGet]
        public Task<ApiResult<List<Mg_mnid>>> Get([FromQuery] Mg_mnid param) =>
            DB.Mg_mnidRepository.Get(param);

        [HttpGet("[action]")]
        public Task<ApiResult<List<Mg_mnid_User>>> GetUser([FromQuery] Mg_mnid param) =>
            DB.Mg_mnidRepository.GetUser(param);

        [HttpGet("[action]")]
        public Task<ApiResult<List<Mg_mnid_Dr>>> GetDr([FromQuery] Mg_mnid param) =>
            DB.Mg_mnidRepository.GetDr(param);

    }
}
