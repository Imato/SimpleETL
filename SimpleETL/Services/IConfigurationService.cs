
using Microsoft.Extensions.Configuration;

namespace SimpleETL
{
    public interface IConfigurationService
    {
        T GetConfiguration<T>();
        IConfiguration GetConfiguration();
    }
}
