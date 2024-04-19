namespace Models
{
    public class FuncCatalogRemoveParam
    {
        /// <summary>
        /// 欲停用的功能目錄階層Id
        /// </summary>
        public string FuncCatalogId { get; set; }

        /// <summary>
        /// FuncCatalogId 所屬的 RootId
        /// </summary>
        public string RootId { get; set; }

        public string MUserId { get; set; }
    }
}
