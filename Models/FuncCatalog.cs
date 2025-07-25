using System.ComponentModel.DataAnnotations;

namespace Models
{
    [Lib.Attributes.Table("FuncCatalog")]
    public class FuncCatalog
    {
        /// <summary>
        /// 功能目錄階層Id
        /// </summary>
        [Key]
        public string FuncCatalogId { get; set; }

        /// <summary>
        /// 根Id
        /// </summary>
        public string RootId { get; set; }

        /// <summary>
        /// 目錄Id
        /// </summary>
        public string CatalogId { get; set; }

        /// <summary>
        /// 功能Id
        /// </summary>
        public string FuncId { get; set; }

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
