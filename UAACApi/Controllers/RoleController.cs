using Lib;
using Lib.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Repositorys;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UAACApi.Controllers
{
    public class RoleController : BaseController
    {
        public RoleController(IOptionsMonitor<AppSettings> settings,
            DBContext db,
            ApiUtilLocator apiUtils,
            UtilLocator utils)
            : base(settings.CurrentValue, db, apiUtils, utils) { }

        [HttpGet]
        public Task<ApiResult<List<Role>>> Get([FromQuery] Role param) =>
            DB.RoleRepository.Get(param);

        [HttpPost]
        public Task<ApiResult<Role>> Insert(Role param) =>
            DB.RoleRepository.Insert(param);

        [HttpPut]
        public Task<ApiResult<Role>> Update(Role param) =>
            DB.RoleRepository.Update(param);

        [HttpPatch]
        public Task<ApiResult<Role>> Patch(object param)
        {
            param.GetModelAndProps(out Role model, out HashSet<string> props);
            model.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            props.AddProps<Role>(m => new { m.MDateTime });
            return DB.RoleRepository.Patch(model, props);
        }
    }
}
