﻿using System.Collections.Generic;

namespace Models
{
    public class FuncGroup : Func
    {
        /// <summary>
        /// 功能目錄階層Id
        /// </summary>
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
        /// 順序
        /// </summary>
        public short? Seq { get; set; }

        /// <summary>
        /// 群組子功能
        /// </summary>
        public List<FuncGroup> Funcs { get; set; } = new();

        /// <summary>
        /// 功能使用者最愛Id
        /// </summary>
        public string FuncUserFavoriteId { get; set; }

        /// <summary>
        /// 使用者最愛
        /// </summary>
        public bool Favorite { get; set; } = false;

    }
}
