
namespace SimpleETL
{
    public class EtlContext
    {
        public IConfigurationService ConfigurationService { get; set; }
        public ILogger Logger { get; set; }

        private object _configuration;
        private object _lock = new object();

        private static EtlContext _context = new EtlContext();

        public static EtlContext GetContext() => _context;        

        public T GetConfiguration<T>()
        {
            lock (_lock)
            {
                if (_configuration == null && ConfigurationService != null)
                    _configuration = ConfigurationService.GetConfiguration<T>();
            }            

            return EtlConverter.TryGetValue<T, object>(_configuration);
        }

        public void SetConfiguration<T>(T configuration)
        {
            if (configuration != null)
                lock (_lock)
                {
                    _configuration = configuration;
                }                
        }

    }
}
