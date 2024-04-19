namespace Params
{
    public class StrParam
    {
        public enum TrimType
        {
            None = 0,
            Trim = 1,
            TrimEnd = 2,
            TrimStart = 3
        }

        public enum CompareResult
        {
            OneLessTwo = -1,
            OneEqualTwo = 0,
            OneMoreTwo = 1,
            FormatErr = 99
        }
    }
}
