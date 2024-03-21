using FastMember;

namespace Imato.SimpleETL
{
    public static class ObjectMapper
    {
        private static Dictionary<Type, MemberSet> _cache = new Dictionary<Type, MemberSet>();

        public static T GetTypedData<T>(this IEtlRow row) where T : new()
        {
            var myData = new T();

            var accessor = TypeAccessor.Create(typeof(T));

            foreach (var m in GetMembers<T>(accessor))
            {
                if (row.HasColumn(m.Name)
                    && row[m.Name] != null)
                {
                    if (m.CanWrite && m.CanRead)
                        accessor[myData, m.Name] = row[m.Name];
                }
            }

            return myData;
        }

        public static IEtlRow AddColumns<T>(this IEtlRow row, object value)
        {
            var accessor = TypeAccessor.Create(typeof(T));

            foreach (var m in GetMembers<T>(accessor))
            {
                if (m.CanRead)
                    row[m.Name] = accessor[value, m.Name];
            }

            return row;
        }

        public static EtlRow GetEtlRow(object value, IEtlDataFlow flow)
        {
            var row = new EtlRow(flow);

            foreach (var f in GetFields(value))
            {
                row[f.Key] = f.Value;
            }

            return row;
        }

        public static IDictionary<string, object> GetFields(object obj)
        {
            var result = new Dictionary<string, object>();
            if (obj != null)
            {
                var type = obj.GetType();
                if (type == typeof(string)
                    || type.IsValueType)
                {
                    result.Add(type.Name.ToLower(), obj);
                    return result;
                }

                if (type.IsClass)
                {
                    var accessor = TypeAccessor.Create(type);
                    foreach (var m in GetMembers(accessor, type))
                    {
                        if (m.CanRead)
                        {
                            result.Add(m.Name, accessor[obj, m.Name]);
                        }
                    }
                }
            }
            return result;
        }

        private static MemberSet GetMembers<T>(TypeAccessor accessor)
        {
            MemberSet members;

            if (_cache.ContainsKey(typeof(T)))
                members = _cache[typeof(T)];
            else
            {
                members = accessor.GetMembers();
                _cache.Add(typeof(T), members);
            }

            return members;
        }

        private static MemberSet GetMembers(TypeAccessor accessor, Type type)
        {
            MemberSet members;

            if (_cache.ContainsKey(type))
                members = _cache[type];
            else
            {
                members = accessor.GetMembers();
                _cache.Add(type, members);
            }

            return members;
        }
    }
}