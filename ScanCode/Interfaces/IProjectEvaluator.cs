using ScanCode.Data;
using System.Collections.Generic;

namespace ScanCode.Interfaces
{
    public interface IProjectEvaluator
    {
        ProjectData CalculateProjectPoint(IList<ActivityRawData> rawData, ComplexityParam param);
        ProjectLevel GetProjectLevel(double totalEffort);
    }
}
