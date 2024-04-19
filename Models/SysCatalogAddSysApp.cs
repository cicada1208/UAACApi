using System.Collections.Generic;

namespace Models
{
    public class SysCatalogAddSysApp
    {
        public string RootId { get; set; }

        public string CatalogId { get; set; }

        public string MUserId { get; set; }

        public List<SysApp> SysApps { get; set; }
    }
}
