namespace Lib
{
    public class UtilLocator
    {
        private ApiUtil _Api;
        public ApiUtil Api =>
            _Api ??= new ApiUtil();

        private CodecUtil _Codec;
        public CodecUtil Codec =>
            _Codec ??= new CodecUtil();

        private CommonUtil _Common;
        public CommonUtil Common =>
            _Common ??= new CommonUtil();

        private ConfigUtil _Config;
        public ConfigUtil Config =>
            _Config ??= new ConfigUtil();

        private DataTableUtil _DataTable;
        public DataTableUtil DataTable =>
            _DataTable ??= new DataTableUtil();

        private DateTimeUtil _DateTime;
        public DateTimeUtil DateTime =>
            _DateTime ??= new DateTimeUtil();

        private HostUtil _Host;
        public HostUtil Host =>
            _Host ??= new HostUtil();

        private LinqUtil _Linq;
        public LinqUtil Linq =>
            _Linq ??= new LinqUtil();

        private MedicalUtil _Medical;
        public MedicalUtil Medical =>
            _Medical ??= new MedicalUtil();

        private ModelUtil _Model;
        public ModelUtil Model =>
            _Model ??= new ModelUtil();

        private RuleUtil _Rule;
        public RuleUtil Rule =>
            _Rule ??= new RuleUtil();

        private SqlBuildUtil _SqlBuild;
        public SqlBuildUtil SqlBuild =>
            _SqlBuild ??= new SqlBuildUtil();

        private StrUtil _Str;
        public StrUtil Str =>
            _Str ??= new StrUtil();

        private TaskUtil _Task;
        public TaskUtil Task =>
            _Task ??= new TaskUtil();

    }
}
