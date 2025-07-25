using Lib;
using Lib.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Models;
using Repositorys;
using Services;
using System.Threading.Tasks;

namespace UAACApi.Controllers
{
    public class AuthController : BaseController
    {
        private readonly AuthService loginService;

        public AuthController(IOptionsMonitor<AppSettings> settings,
            DBContext db,
            ApiUtilLocator apiUtils,
            UtilLocator utils,
            AuthService loginService)
            : base(settings.CurrentValue, db, apiUtils, utils)
        {
            this.loginService = loginService;
        }

        /// <summary>
        /// 查詢使用者資訊
        /// </summary>
        [HttpGet("[action]/{userId}")]
        public Task<ApiResult<Auth>> GetUserInfo(string userId) =>
            loginService.GetUserInfo(userId);

        /// <summary>
        /// 驗證及查詢使用者資訊
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> PostLogin()
        {
            var result = await loginService.PostLogin(Request);
            return new JsonResult(result) { StatusCode = (int?)result.Code };
        }

    }
}
