using System.Collections.Generic;
using System.ComponentModel;

namespace ScanCode.Data
{
    public class ProjectData : ProjectLevel, INotifyPropertyChanged
    {
        public double TotalEffort { get; set; }
        public int TotalActivities { get; set; }
        public int TotalConditions { get; set; }
        public int TotalWorkflows { get; set; }
        public Complexity ComplexityRank { get; set; }
        public string ProjectName { get; set; }

        private bool _enabled = true;
        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value)
                {
                    return;
                }
                _enabled = value;
                OnPropertyChanged(nameof(Enabled));
            }
        }

        public IList<ActivityRawData> ScanResult { get; set; } = new List<ActivityRawData>();

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
