using static Params.FuncParam;

namespace Models
{
    public class SysFuncPermissionDetail : SysFuncPermission
    {
        /// <summary>
        /// 功能名稱
        /// </summary>
        public string FuncName { get; set; }

        /// <summary>
        /// 功能類別
        /// </summary>
        public FuncType? FuncType { get; set; }

        /// <summary>
        /// 基礎路徑或完整路徑
        /// </summary>
        public string BasePath { get; set; }

        /// <summary>
        /// 子路徑
        /// </summary>
        public string SubPath { get; set; }

        /// <summary>
        /// 檔名(含附檔名)
        /// </summary>
        public string Assembly { get; set; }

        /// <summary>
        /// 視圖名稱
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// 視圖頁面元件
        /// </summary>
        public string ViewComponent { get; set; }

        /// <summary>
        /// 圖示類型
        /// </summary>
        public short? IconType { get; set; }

        /// <summary>
        /// 圖示內容或路徑
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 限制執行次數
        /// </summary>
        public short? Limit { get; set; }
    }
}
