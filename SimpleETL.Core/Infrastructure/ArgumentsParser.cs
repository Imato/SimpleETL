using System;
using System.Linq;

namespace SimpleETL
{
    public class ArgumentsParser
    {
        private string[] _args;

        public ArgumentsParser(string[] args = null)
        {
            _args = args;
        }

        public object Get(string argumentName)
        {
            if(_args == null 
                || _args.Length == 0
                || !_args.Contains(argumentName))
            {
                return null;
            }

            var index = Array.IndexOf(_args, argumentName);

            if (_args.Length == index + 1)
            {
                return true;
            }

            if (_args.Length > index + 1 
                && _args[index + 1].StartsWith("-"))
            {
                return true;
            }

            return _args[index + 1];
        }
    }
}
