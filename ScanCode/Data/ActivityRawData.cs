using System.Collections.Generic;

namespace ScanCode.Data
{
    public class ActivityRawData
    {
        public ActType ActivityType { get; set; }
        public string WorkflowName { get; set; }
        public bool IsAvailable { get; set; }
        public string App { get; set; }
        public double ActivityPts { get; set; }
        public double AppPts { get; set; }
        public bool IsConditional { get; set; }
        public int Count { get; set; } = 1;
        public Dictionary<string, object> Properties = new Dictionary<string, object>();
    }
}
