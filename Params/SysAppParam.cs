namespace Params
{
    public class SysAppParam
    {
        /// <summary>
        /// 值與 Func.FuncType 對應
        /// </summary>
        public enum SysType
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
            HISExe = 5
        }

        public class SysId
        {
            /// <summary>
            /// 系統目錄階層
            /// </summary>
            public const string BasicRoot = "BasicRoot";

            public const string Portal = "Portal";
            /// <summary>
            /// 電子病歷
            /// </summary>
            public const string iEMR = "iEMR";
            /// <summary>
            /// 嘉基資訊系統
            /// </summary>
            public const string CychMis = "CychMis";
            /// <summary>
            /// 電子表單
            /// </summary>
            public const string CychWeb = "CychWeb";
            /// <summary>
            /// 醫療系統
            /// </summary>
            public const string HIS = "HIS";
            /// <summary>
            /// 排檢系統
            /// </summary>
            public const string CychPeri = "CychPeri";
        }

    }
}
