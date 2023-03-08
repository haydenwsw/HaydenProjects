using System;
using System.Collections.Generic;
using System.Text;

namespace CarSearcher.Exceptions
{
    [Serializable]
    public class FbMarketplaceException : Exception
    {
        public FbMarketplaceException() { }

        public FbMarketplaceException(string message)
            : base(message) { }

        public FbMarketplaceException(string message, Exception inner)
            : base(message, inner) { }
    }
}
