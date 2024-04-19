namespace Models
{
    public class LoggerData
    {
        public string RequestIP { get; set; }

        public string Url { get; set; }

        public string Method { get; set; }

        public string ControllerName { get; set; }

        public string ActionName { get; set; }

        public string QueryString { get; set; }

        public string Body { get; set; }

        public string FormData { get; set; }

        public string Cookies { get; set; }
        public string Headers { get; set; }

        public object ActionArguments { get; set; }

        public string ActionException { get; set; }

        public string ResponseBody { get; set; }

        public int StatusCode { get; set; }

        public string LoggerException { get; set; }
    }
}
