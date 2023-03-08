using System;
using System.Collections.Generic;
using System.Text;

namespace CarSearcher.Exceptions
{
    [Serializable]
    public class GumtreeException : Exception
    {
        public GumtreeException() { }

        public GumtreeException(string message)
            : base(message) { }

        public GumtreeException(string message, Exception inner)
            : base(message, inner) { }
    }
}
