using Newtonsoft.Json;
using ScanCode.Data;
using System.Collections.Generic;
using System.IO;

namespace ScanCode.Utils
{
    public class ConditionalActivities
    {
        private static ISet<ActType> _conditionalActivities;

        private static ISet<ActType> LoadConditionalActivities()
        {
            return JsonConvert.DeserializeObject<HashSet<ActType>>(File.ReadAllText($"{Constants.ConfigFolder}\\ConditionalActivities.json"));
        }

        public static bool Check(ActType activityType)
        {
            if (_conditionalActivities == null)
            {
                _conditionalActivities = LoadConditionalActivities();
            }
            return _conditionalActivities.Contains(activityType);
        }
    }
}
