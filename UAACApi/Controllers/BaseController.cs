using Lib;
using Lib.Api;
using Lib.Api.Attributes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Repositorys;

namespace UAACApi.Controllers
{
    [Route("[controller]")]
    [Produces("application/json")]
    [ApiController]
    [ApiActionFilter]
    [ApiExceptionFilter]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public abstract class BaseController : ControllerBase
    {
        public BaseController(AppSettings settings, DBContext db, ApiUtilLocator apiUtils, UtilLocator utils)
        {
            Settings = settings;
            DB = db;
            ApiUtils = apiUtils;
            Utils = utils;
        }

        protected AppSettings Settings { get; }

        protected DBContext DB { get; }

        protected ApiUtilLocator ApiUtils { get; }

        protected UtilLocator Utils { get; }

        protected string RemoteIpAddress =>
            HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

    }
}
