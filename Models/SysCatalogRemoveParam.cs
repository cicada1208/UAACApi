namespace Models
{
    public class SysCatalogRemoveParam
    {
        /// <summary>
        /// 欲停用的系統目錄階層Id
        /// </summary>
        public string SysCatalogId { get; set; }

        /// <summary>
        /// SysCatalogId 所屬的 RootId
        /// </summary>
        public string RootId { get; set; }

        public string MUserId { get; set; }
    }
}
