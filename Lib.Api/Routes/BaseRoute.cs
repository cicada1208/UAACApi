using Microsoft.Extensions.Configuration;

namespace Lib.Api.Routes
{
    /// <summary>
    /// API Name (API Url 定義於 appsettings.json)
    /// </summary>
    public class ApiName
    {
        /// <summary>
        /// 手術系統
        /// </summary>
        public const string OPState = "OPState";
        /// <summary>
        /// 人資系統
        /// </summary>
        public const string Hr = "Hr";
    }

    public class BaseRoute
    {
        /// <summary>
        /// API Url
        /// </summary>
        public static string Service(string name)
        {
            IConfigurationRoot configuration = ConfigUtil.ConfigAppSettings();
            return configuration.GetValue<string>($"ApiUrl:{name}");
        }
    }
}
