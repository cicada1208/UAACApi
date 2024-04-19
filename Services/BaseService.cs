using Lib;
using Lib.Api;
using Models;
using Repositorys;

namespace Services
{
    public abstract class BaseService
    {
        public BaseService(AppSettings settings, DBContext db, ApiUtilLocator apiUtils, UtilLocator utils)
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

    }
}
