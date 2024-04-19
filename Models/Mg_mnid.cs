using Params;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    [Lib.Attributes.Table(DBParam.DBType.SYBASE, DBParam.DBName.SYB2, "mg_mnid")]
    public class Mg_mnid 
    {
        [Key]
        public string nid_id { get; set; }

        [Key]
        public string nid_code { get; set; }

        public string nid_trn { get; set; }

        public string nid_name { get; set; }

        public string nid_rec { get; set; }

    }
}
