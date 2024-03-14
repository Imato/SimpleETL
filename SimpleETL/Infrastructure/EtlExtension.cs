using System.Threading.Tasks;

namespace SimpleETL
{
    public static class EtlExtension
    {
        public static EtlObject SetParent(this EtlObject obj, EtlObject parent)
        {
            obj.ParentEtl = parent;
            return obj;
        }
    }
}
