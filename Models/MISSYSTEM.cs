using System.ComponentModel.DataAnnotations;

namespace Models
{
    [Lib.Attributes.Table("MISSYSTEM")]
    public class MISSYSTEM
    {
        [Key]
        public string SYSID { get; set; }

        public string SYSNAME { get; set; }

        public string EXENAME { get; set; }

        public string FOLDERNAME { get; set; }

        public string LOCALPATH { get; set; }

        public string SERVERPATH { get; set; }

        public string SUBSYS { get; set; }

        public string ISSTOP { get; set; }

        public string depnoAuth { get; set; }

    }
}

