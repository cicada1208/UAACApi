using Lib;
using Params;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Params.SysAppParam;

namespace Models
{
    [Lib.Attributes.Table("SysApp")]
    public class SysApp
    {
        /// <summary>
        /// 系統Id
        /// </summary>
        [Key]
        public string SysId { get; set; }

        /// <summary>
        /// 系統名稱
        /// </summary>
        public string SysName { get; set; }

        /// <summary>
        /// 系統類別
        /// </summary>
        public SysType? SysType { get; set; }

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
                switch (SysType)
                {
                    case SysAppParam.SysType.VersionExe:
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