using System.Collections.Generic;

namespace ScanCode.Data
{
    public class AppActivity
    {
        public ActType Activity { get; set; }
        public IList<ActType> ChildNodes { get; set; }
        public IList<string> ChildAttributes { get; set; }

    }
}
