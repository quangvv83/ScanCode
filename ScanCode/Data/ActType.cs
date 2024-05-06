namespace ScanCode.Data
{
    public class ActType
    {
        public string Name { get; set; }
        public string Namespace { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null || this == null)
            {
                return false;
            }
            if (!(obj is ActType at))
            {
                return false;
            }
            return Name == at.Name && Namespace == at.Namespace;
        }

        public override int GetHashCode()
        {
            return (Name + "-" + Namespace).GetHashCode();
        }
    }
}
