namespace Models
{
    public class AppSettings
    {
        public ConnectionStrings ConnectionStrings { get; set; }

        public Jwt Jwt { get; set; }

        public Logging Logging { get; set; }

        public string AllowedHosts { get; set; }
    }

    public class ConnectionStrings
    {
        //public string NIS { get; set; }
        public string NISDB { get; set; }
        public string SYB1 { get; set; }
        public string SYB2 { get; set; }
        public string PeriExam { get; set; }
    }

    public class Jwt
    {
        public string Issuer { get; set; }
        public string SigningKey { get; set; }
    }

    public class Logging
    {
        public Loglevel LogLevel { get; set; }
    }

    public class Loglevel
    {
        public string Default { get; set; }
    }

}
