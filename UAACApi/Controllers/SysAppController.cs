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
    public class SysAppController : BaseController
    {
        public SysAppController(IOptionsMonitor<AppSettings> settings,
            DBContext db,
            ApiUtilLocator apiUtils,
            UtilLocator utils)
            : base(settings.CurrentValue, db, apiUtils, utils) { }

        [HttpGet]
        public Task<ApiResult<List<SysApp>>> Get([FromQuery] SysApp param) =>
            DB.SysAppRepository.Get(param);

        /// <summary>
        /// 查詢系統目錄階層的加入清單
        /// </summary>
        [HttpGet("[action]")]
        public Task<ApiResult<List<SysApp>>> GetSysCatalogAddList([FromQuery] SysCatalogQueryParam param) =>
            DB.SysAppRepository.GetSysCatalogAddList(param);

        /// <summary>
        /// 查詢系統目錄階層的排序清單
        /// </summary>
        [HttpGet("[action]")]
        public Task<ApiResult<List<SysGroup>>> GetSysCatalogSortList([FromQuery] SysCatalogQueryParam param) =>
            DB.SysAppRepository.GetSysCatalogSortList(param);

        [HttpPost]
        public Task<ApiResult<SysApp>> Insert(SysApp param) =>
            DB.SysAppRepository.Insert(param);

        [HttpPut]
        public Task<ApiResult<SysApp>> Update(SysApp param) =>
            DB.SysAppRepository.Update(param);

        [HttpPatch]
        public Task<ApiResult<SysApp>> Patch(object param)
        {
            param.GetModelAndProps(out SysApp model, out HashSet<string> props);
            model.MDateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            props.AddProps<SysApp>(m => new { m.MDateTime });
            return DB.SysAppRepository.Patch(model, props);
        }
    }
}
