using ScanCode.Data;
using ScanCode.Interfaces;
using ScanCode.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using ScanCode.Extensions;

namespace ScanCode.Services
{
    public class WorkflowReader : IWorkflowReader
    {
        public string ProjectName { get; private set; }
        public int ConditionNumber { get; private set; }
        public int MaxWorkflowLevel { get; private set; }
        public IList<ActivityRawData> ScanResult { get; private set; } = new List<ActivityRawData>();
        public ISet<string> Workflows { get; private set; } = new HashSet<string>();
        public IDictionary<string, string> Dependencies { get; private set; } = new Dictionary<string, string>();
        public ISet<ActType> NotAvailableActivities { get; private set; } = new HashSet<ActType>();
        public ISet<string> Namespaces { get; private set; } = new HashSet<string>();
        public ISet<string> References { get; private set; } = new HashSet<string>();

        // a set of invoked workflow, to prevent scanning a workflow to many times
        private ISet<string> InvokedWorkflows { get; set; } = new HashSet<string>();

        public void Scan(string rootFolder)
        {
            // clear previous data
            ClearData();

            // scan project
            foreach (var file in Directory.GetFiles(rootFolder, "*.xaml", SearchOption.AllDirectories))
            {
                if (File.Exists(file))
                {
                    try
                    {
                        XElement rootElement = XElement.Load(file);
                        ScanFolderStruct(rootElement, file, 1, 1, rootFolder);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"While scanning file: {file}", e);
                    }
                }
            }

            // Get project name, dependencies(installed packages)
            string settingFile = "project.json";
            string projectLocation = Path.Combine(rootFolder, settingFile);
            var projectInfo = ReadProjectInfo(projectLocation);
            Dependencies = projectInfo.Dependencies;
            ProjectName = projectInfo.Name;
        }

        public ProjectInfo ReadProjectInfo(string projectLocation)
        {
            if (File.Exists(projectLocation))
            {
                var settingText = File.ReadAllText(projectLocation);
                var projectInfo = JsonConvert.DeserializeObject<ProjectInfo>(settingText);
                return projectInfo;
            }
            return null;
        }

        private void ClearData()
        {
            ConditionNumber = 0;
            MaxWorkflowLevel = 0;
            MaxWorkflowLevel = 0;
            ScanResult.Clear();
            Workflows.Clear();
            Dependencies.Clear();
            NotAvailableActivities.Clear();
            InvokedWorkflows.Clear();
        }

        private void ScanFolderStruct(XElement el, string mainPath, int level, int workflowID, string rootFolder, string app = null)
        {
            if (!File.Exists(mainPath))
            {
                return;
            }

            // Add to workflow set
            Workflows.Add(GetWorkflowName(mainPath));
            MaxWorkflowLevel = Math.Max(MaxWorkflowLevel, level);

            string appName = app;

            // Check each sub-element
            foreach (XElement subEl in el.Elements())
            {
                // if activity then
                if (IsActivity(subEl))
                {
                    var activityType = new ActType { Name = subEl.Name.LocalName, Namespace = subEl.Name.NamespaceName };
                    bool available = Points.CheckAvailability(activityType);
                    bool isConditional = ConditionalActivities.Check(activityType);
                    appName = string.IsNullOrEmpty(app) ? Points.GetAppOfActivity(subEl) : app;

                    ActivityRawData activityRawData = new ActivityRawData
                    {
                        ActivityType = activityType,
                        ActivityPts = Points.GetActivityPts(activityType),
                        IsAvailable = available,
                        WorkflowName = GetWorkflowName(mainPath),
                        App = appName,
                        AppPts = Points.GetAppPoint(appName),
                        IsConditional = isConditional,
                    };
                    GetPropertiesOfActivity(subEl, activityRawData);
                    ScanResult.Add(activityRawData);

                    if (!available)
                    {
                        NotAvailableActivities.Add(activityType);
                    }
                    if (isConditional)
                    {
                        ConditionNumber++;
                    }
                }
                else if (IsNamespace(subEl))
                {
                    var collectionElement = subEl.Elements().FirstOrDefault(e => e.Name.LocalName == "List");
                    if (collectionElement != null)
                    {
                        foreach (var item in collectionElement.Elements())
                        {
                            Namespaces.Add(item.Value);
                        }
                    }
                }
                else if (IsReference(subEl))
                {
                    var collectionElement = subEl.Elements().FirstOrDefault(e => e.Name.LocalName == "List");
                    if (collectionElement != null)
                    {
                        foreach (var item in collectionElement.Elements())
                        {
                            References.Add(item.Value);
                        }
                    }
                }

                if (subEl.Elements().Count() > 0)
                {
                    // scan all activities in the comment out
                    ScanFolderStruct(subEl, mainPath, level, workflowID, rootFolder, appName);
                }

            }
        }

        private bool IsActivity(XElement el)
        {
            var actType = new ActType
            {
                Name = el.Name.LocalName,
                Namespace = el.Name.NamespaceName
            };
            if (Points.CheckAvailability(actType))
            {
                return true;
            }
            if (Points.CheckActivityByTypeAndXmlns(actType.Namespace, actType.Name))
            {
                return true;
            }

            return false;
        }

        private bool IsNamespace(XElement el)
        {
            return el.Name.LocalName == Constants.NamespacesForImplementationTag;
        }

        private bool IsReference(XElement el)
        {
            return el.Name.LocalName == Constants.ReferencesForImplementationTag;
        }

        private void GetPropertiesOfActivity(XElement el, ActivityRawData activityRawData)
        {
            el.ForeachAttribute(a =>
            {
                if (!activityRawData.Properties.ContainsKey(a.Name.LocalName))
                {
                    activityRawData.Properties.Add(a.Name.LocalName, a.Value);
                }
            });

            var childProps = el.Elements().Where(e => e.Name.Namespace == el.Name.Namespace && e.Name.LocalName.Contains($"{el.Name.LocalName}."));
            foreach (var item in childProps)
            {
                if (item.Name.LocalName == $"{el.Name.LocalName}.Variables")
                {

                }
                else
                {

                }
            }
        }

        private string GetWorkflowName(string path)
        {
            return Path.GetFileName(path);
        }

    }
}
