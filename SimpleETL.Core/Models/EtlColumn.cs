using System;

namespace SimpleETL
{
    public class EtlColumn
    {
        public int Id { get; }
        public string Name { get; }
        public Type Type { get; private set;  }

        public EtlColumn(int id, string name, Type type)
        {
            Id = id;
            Name = name;
            Type = type ?? typeof(object);
        }

        public void Set(Type type)
        {
            Type = type;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var o = obj as EtlColumn;
            return o == null ? false : o.Name == Name;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
