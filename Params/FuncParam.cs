namespace Params
{
    public class FuncParam
    {
        /// <summary>
        /// 值與 SysApp.SysType 對應
        /// </summary>
        public enum FuncType
        {
            /// <summary>
            /// 根
            /// </summary>
            Root = 0,
            /// <summary>
            /// 目錄
            /// </summary>
            Catalog = 1,
            /// <summary>
            /// 網址
            /// </summary>
            Site = 2,
            /// <summary>
            /// 分版次執行檔
            /// </summary>
            VersionExe = 3,
            /// <summary>
            /// 嘉基資訊系統執行檔
            /// </summary>
            CychMisExe = 4,
            /// <summary>
            /// 醫療系統執行檔
            /// </summary>
            HISExe = 5,
            /// <summary>
            /// WPF Page
            /// </summary>
            WpfPage = 6,
            /// <summary>
            /// WPF Window
            /// </summary>
            WpfWindow = 7,
            /// <summary>
            /// Vue Page
            /// </summary>
            VuePage = 8,
            /// <summary>
            /// WPF Method
            /// </summary>
            WpfMethod = 9
        }

        public class FuncId
        {
            /// <summary>
            /// Portal 功能目錄階層
            /// </summary>
            public const string PortalRoot = "PortalRoot";
        }

    }
}
