using System.Collections.Generic;
using System.ComponentModel;

namespace ScanCode.Data
{
    public class ProjectInfo : INotifyPropertyChanged
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public IDictionary<string, string> Dependencies { get; set; }
        public bool Enabled { get; set; } = true;

        private int _index;
        public int Index
        {
            get => _index;
            set
            {
                if (_index == value)
                {
                    return;
                }
                _index = value;
                OnPropertyChanged(nameof(Index));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override bool Equals(object obj)
        {
            if (this == null || obj == null)
            {
                return false;
            }
            if (!(obj is ProjectInfo pi))
            {
                return false;
            }
            return Location.Equals(pi.Location);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

    }
}
