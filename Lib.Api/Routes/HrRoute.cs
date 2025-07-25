namespace Lib.Api.Routes
{
    public class HrRoute : BaseRoute
    {
        /// <summary>
        /// API Url
        /// </summary>
        public static string Service() => Service(ApiName.Hr);

        public class SearchEmp
        {
            public const string UserInfos = "SearchEmp/UserInfos/";
        }

        public class BasicInfo
        {
            public const string UserInfosV2 = "BasicInfo/V2/";
        }

    }
}
