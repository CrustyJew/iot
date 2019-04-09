using System;
using System.Collections.Generic;
using System.Text;

namespace Iot.Units
{
    public class UnknownConversionException : Exception
    {
        public UnknownConversionException(string message) : base(message)
        {
        }
    }
}
