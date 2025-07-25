using System.ComponentModel.DataAnnotations;

namespace Models
{
    [Lib.Attributes.Table("SysCatalog")]
    public class SysCatalog
    {
        /// <summary>
        /// 系統目錄階層Id
        /// </summary>
        [Key]
        public string SysCatalogId { get; set; }

        /// <summary>
        /// 根Id
        /// </summary>
        public string RootId { get; set; }

        /// <summary>
        /// 目錄Id
        /// </summary>
        public string CatalogId { get; set; }

        /// <summary>
        /// 系統Id
        /// </summary>
        public string SysId { get; set; }

        /// <summary>
        /// 順序
        /// </summary>
        public short? Seq { get; set; }

        /// <summary>
        /// 啟用
        /// </summary>
        public bool? Activate { get; set; }

        public string MUserId { get; set; }

        public string MDateTime { get; set; }

    }
}
