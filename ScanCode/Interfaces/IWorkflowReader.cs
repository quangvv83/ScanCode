using ScanCode.Data;
using System.Collections.Generic;

namespace ScanCode.Interfaces
{
    public interface IWorkflowReader
    {
        string ProjectName { get; }
        int ConditionNumber { get; }
        int MaxWorkflowLevel { get; }
        IList<ActivityRawData> ScanResult { get; }
        ISet<string> Workflows { get; }
        ISet<string> Namespaces { get; }
        ISet<string> References { get; }
        IDictionary<string, string> Dependencies { get; }
        ISet<ActType> NotAvailableActivities { get; }

        ProjectInfo ReadProjectInfo(string projectLocation);
        void Scan(string path);
    }
}
