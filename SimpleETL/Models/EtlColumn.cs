namespace Imato.SimpleETL
{
    public class EtlColumn
    {
        public int Id { get; init; }
        public string Name { get; init; }
        public Type Type { get; set; }

        public override int GetHashCode()
        {
            return Name.GetHashCode() + 4424245;
        }

        public override bool Equals(object obj)
        {
            return (obj as EtlColumn)?.Name == Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}