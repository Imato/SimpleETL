using System;
using System.Text;

namespace SimpleETL
{
    public static class ExceptionUtils
    {
        public static string ToLogString(this Exception exception)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Exception:");
            sb.AppendLine(exception.Message);
            sb.AppendLine("Trace:");
            sb.AppendLine(exception.StackTrace);

            while (exception.InnerException != null)
            {
                exception = exception.InnerException;
                sb.AppendLine("Inner Exception:");
                sb.AppendLine(exception.Message);
                sb.AppendLine("Inner Exception Trace:");
                sb.AppendLine(exception.StackTrace);
            }

            return sb.ToString();
        }

        
    }
}
