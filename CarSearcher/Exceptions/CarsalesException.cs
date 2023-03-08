using System;
using System.Collections.Generic;
using System.Text;

namespace CarSearcher.Exceptions
{
    [Serializable]
    class CarsalesException : Exception
    {
        public CarsalesException() { }

        public CarsalesException(string message)
            : base(message) { }

        public CarsalesException(string message, Exception inner)
            : base(message, inner) { }
    }
}
