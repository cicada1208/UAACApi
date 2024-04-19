using System.Collections.Generic;

namespace Models
{
    public class SysGroup : SysApp
    {
        /// <summary>
        /// 系統目錄階層Id
        /// </summary>
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
        /// 順序
        /// </summary>
        public short? Seq { get; set; }

        /// <summary>
        /// 群組子系統
        /// </summary>
        public List<SysGroup> SysApps { get; set; } = new();

        /// <summary>
        /// 系統使用者最愛Id
        /// </summary>
        public string SysUserFavoriteId { get; set; }

        /// <summary>
        /// 使用者最愛
        /// </summary>
        public bool Favorite { get; set; } = false;

    }
}
