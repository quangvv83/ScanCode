using ScanCode.Data;
using ScanCode.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using ScanCode.Utils;

namespace ScanCode.Services
{
    public class Evaluator : IProjectEvaluator
    {
        private IList<ProjectLevel> _projectLevels = null;
        private IList<Complexity> _complexityRanks = null;

        public ProjectData CalculateProjectPoint(IList<ActivityRawData> rawData, ComplexityParam param)
        {
            // get project size (man day)
            double ps = CalculateProjectSize(rawData);
            // get project complexity
            Complexity complexity = CalculateComplexity(param);
            // calculate total effort
            double totalEffort = CalculateTotalEffort(ps, complexity.Point);

            var level = GetProjectLevel(totalEffort);
            return new ProjectData
            {
                Ceiling = level.Ceiling,
                Floor = level.Floor,
                Level = level.Level,
                Color = level.Color,
                TotalEffort = totalEffort,
                TotalActivities = rawData.Count,
                TotalConditions = param.ConditionNumber,
                TotalWorkflows = param.WorkflowNumber,
                ComplexityRank = complexity,
            };
        }

        public ProjectLevel GetProjectLevel(double totalEffort)
        {
            var projectLevels = LoadProjectLevels();
            var level = projectLevels.FirstOrDefault(p => totalEffort >= p.Floor && totalEffort < p.Ceiling);
            return level;
        }

        private double CalculateProjectSize(IList<ActivityRawData> rawData)
        {
            double ps = rawData.Sum(d => d.ActivityPts * d.AppPts);
            return ps;
        }

        private Complexity CalculateComplexity(ComplexityParam param)
        {
            // calculate complexity point from param
            var cp = param.ConditionNumber * 0.7 + param.WorkflowNumber * 0.3;

            // get the complexity level
            var complexityRanks = LoadComplexityRank();
            var rank = complexityRanks.FirstOrDefault(r => cp >= r.Floor && cp < r.Ceiling);
            if (rank == null)
            {
                rank = complexityRanks.LastOrDefault();
            }

            return rank;
        }

        private double CalculateTotalEffort(double projectSize, double complexity)
        {
            var implementationStages = LoadImplementationStages();
            double totalEffort = 0;

            foreach (var key in implementationStages.Keys)
            {
                totalEffort += projectSize * implementationStages[key] * (key != implementationStages.Keys.FirstOrDefault() ? complexity : 1);
            }
            return totalEffort;
        }

        private IDictionary<string, double> LoadImplementationStages()
        {
            return JsonConvert.DeserializeObject<Dictionary<string, double>>(File.ReadAllText($"{Constants.ConfigFolder}\\ImplementationStages.json"));
        }

        private IList<Complexity> LoadComplexityRank()
        {
            if (_complexityRanks == null)
            {
                _complexityRanks = JsonConvert.DeserializeObject<IList<Complexity>>(File.ReadAllText($"{Constants.ConfigFolder}\\ComplexityRank.json"));
            }
            return _complexityRanks;
        }

        private IList<ProjectLevel> LoadProjectLevels()
        {
            if (_projectLevels == null)
            {
                _projectLevels = JsonConvert.DeserializeObject<IList<ProjectLevel>>(File.ReadAllText($"{Constants.ConfigFolder}\\ProjectLevel.json"));
            }
            return _projectLevels;
        }

    }
}
