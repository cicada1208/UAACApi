using System.ComponentModel.DataAnnotations;

namespace Models
{
    [Lib.Attributes.Table("mg_mnid")]
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
