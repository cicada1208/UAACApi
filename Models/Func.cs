using Lib;
using Params;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Params.FuncParam;

namespace Models
{
    [Lib.Attributes.Table("Func")]
    public class Func
    {
        /// <summary>
        /// 功能Id
        /// </summary>
        [Key]
        public string FuncId { get; set; }

        /// <summary>
        /// 系統Id
        /// </summary>
        public string SysId { get; set; }

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
        /// <para>VersionExe：基礎路徑 + 子路徑</para>
        /// <para>Other：基礎路徑</para>
        /// </summary>
        [NotMapped]
        public string Path
        {
            get
            {
                switch (FuncType)
                {
                    case FuncParam.FuncType.VersionExe:
                        if (BasePath.NullableToStr().StartsWith("\\"))
                            return $@"{BasePath.NullableToStr().TrimEnd('\\')}{(BasePath.IsNullOrWhiteSpace() || SubPath.IsNullOrWhiteSpace() ? "" : @"\")}{SubPath.NullableToStr().Trim('\\')}";
                        else  // http(s)
                            return $@"{BasePath.NullableToStr().TrimEnd('/')}{(BasePath.IsNullOrWhiteSpace() || SubPath.IsNullOrWhiteSpace() ? "" : "/")}{SubPath.NullableToStr().Trim('/')}";
                    default:
                        return BasePath.NullableToStr();
                }
            }
        }

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

        /// <summary>
        /// 啟用
        /// </summary>
        public bool? Activate { get; set; }

        public string MUserId { get; set; }

        public string MDateTime { get; set; }

    }
}
