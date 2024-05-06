using ScanCode.Utils;

namespace ScanCode.Data
{
    public class ProjectLevel
    {
        public string Level { get; set; }
        public double Floor { get; set; }
        public double Ceiling { get; set; }
        public string Color { get; set; }
        public string EffortRange => Ceiling < Constants.MaxMD ? $"{Floor}-{Ceiling}" : $"> {Floor}";
        public string ProjectPoint => Ceiling < Constants.MaxMD ? $"{Floor / Constants.DefaultWorkingDayOfMonth}-{Ceiling / Constants.DefaultWorkingDayOfMonth}" : $"> {Floor / Constants.DefaultWorkingDayOfMonth}";
    }
}
