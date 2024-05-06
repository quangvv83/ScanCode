using Newtonsoft.Json;
using ScanCode.Data;
using ScanCode.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace ScanCode.Utils
{
    public class Points
    {
        private static IDictionary<ActType, double> _activityPointTable;
        private static Assembly _systemActivityAssembly;
        private static IList<Assembly> _allAppAssemblies;
        private static IList<AppActivity> _appActivities;

        public static double GetActivityPts(ActType activityType)
        {
            if (_activityPointTable == null)
            {
                _activityPointTable = LoadActivityPoints();
            }

            if (_activityPointTable.TryGetValue(activityType, out double p))
            {
                return p;
            }

            if (_activityPointTable.TryGetValue(new ActType { Name = Constants.DefaultType, Namespace = Constants.DefaultNamespace }, out double dp))
            {
                return dp;
            }

            return 0.03;
        }

        private static IDictionary<ActType, double> LoadActivityPoints()
        {
            var json = File.ReadAllText($"{Constants.ConfigFolder}\\ActivityPoints.json");
            Dictionary<string, double> dictionary = JsonConvert.DeserializeObject<Dictionary<string, double>>(json);

            Dictionary<ActType, double> objectDictionary = new Dictionary<ActType, double>();
            foreach (var kvp in dictionary)
            {
                string keyJson = kvp.Key;
                ActType keyActType = null;
                if (keyJson.StartsWith("{") && keyJson.EndsWith("}"))
                {
                    keyActType = JsonConvert.DeserializeObject<ActType>(keyJson);
                    objectDictionary.Add(keyActType, kvp.Value);
                }
            }

            return objectDictionary;
        }

        private static IList<AppActivity> LoadAppActivities()
        {
            var json = File.ReadAllText($"{Constants.ConfigFolder}\\AppActivities.json");
            var activities = JsonConvert.DeserializeObject<List<AppActivity>>(json);
            return activities;
        }

        public static bool CheckAvailability(ActType activityType)
        {
            if (_activityPointTable == null)
            {
                _activityPointTable = LoadActivityPoints();
            }
            return _activityPointTable.ContainsKey(activityType);
        }

        public static bool CheckActivityByTypeAndXmlns(string xmlns, string typeName)
        {
            // try to load System.Activities assembly
            var someType = typeof(System.Activities.Statements.Sequence);

            // Load all assemblies in the current application domain
            if (_allAppAssemblies == null)
            {
                _allAppAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            }

            // Find the assembly containing the custom attribute of the XML namespace
            if (_systemActivityAssembly == null)
            {
                _systemActivityAssembly = _allAppAssemblies.FirstOrDefault(a => a.GetCustomAttributes<System.Windows.Markup.XmlnsDefinitionAttribute>().Any(attr => attr.XmlNamespace == xmlns));
                if (_systemActivityAssembly == null)
                {
                    return false;
                }
            }

            // find all namespaces corresponding to the xmlns
            var namespaces = _systemActivityAssembly.GetCustomAttributes<System.Windows.Markup.XmlnsDefinitionAttribute>().Where(a => a.XmlNamespace == xmlns).Select(attr => attr.ClrNamespace);
            foreach (var ns in namespaces)
            {
                Type type = _systemActivityAssembly.GetType($"{ns}.{typeName}");
                if (type != null)
                {
                    return type.IsSubclassOf(typeof(System.Activities.Activity));
                }
            }

            return false;
        }

        public static string GetAppOfActivity(XElement el)
        {
            if (_appActivities == null)
            {
                _appActivities = LoadAppActivities();
            }

            // find app activity from pre-defined list
            var appActivity = _appActivities.FirstOrDefault(a => a.Activity.Name == el.Name.LocalName && a.Activity.Namespace == el.Name.NamespaceName);
            if (appActivity == null)
            {
                return string.Empty;
            }

            // get the XElement
            var xElement = el;
            foreach (var item in appActivity.ChildNodes)
            {
                xElement = xElement.Elements(XName.Get(item.Name, item.Namespace)).FirstOrDefault();
                if (xElement == null)
                {
                    return string.Empty;
                }
            }

            // get the XAttribute
            XAttribute xAttribute = null;
            for (int i = 0; i < appActivity.ChildAttributes.Count; i++)
            {
                if (xAttribute == null)
                {
                    xAttribute = xElement.GetAttribute(appActivity.ChildAttributes[i]);
                }
                else
                {
                    var childElement = XElement.Parse(xAttribute.Value);
                    xAttribute = childElement.GetAttribute(appActivity.ChildAttributes[i]);
                }
            }

            // return app name
            if (xAttribute == null)
            {
                return string.Empty;
            }
            if (xAttribute.Value == Constants.XamlNullValue)
            {
                return Constants.DefaultBrowser;
            }
            return xAttribute.Value;

        }

        public static double GetAppPoint(string app)
        {
            return 1;
        }

    }

}
