namespace Models
{
    [Lib.Attributes.Table("SysLog")]
    public class SysLog
    {
        public string LogDateTime { get; set; }

        public string UserId { get; set; }

        public string UserIP { get; set; }

        public string SysId { get; set; }

        public string ProcId { get; set; }

        public string ActionType { get; set; }

        public string ActionTarget { get; set; }

        public string ControllerClass { get; set; }

        public string ActionMethod { get; set; }

        public bool? State { get; set; }

        public string Msg { get; set; }

    }
}
