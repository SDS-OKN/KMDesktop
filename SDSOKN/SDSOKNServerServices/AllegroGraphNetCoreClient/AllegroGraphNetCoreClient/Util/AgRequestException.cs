using System;

namespace AllegroGraphNetCoreClient.Util
{
    public class AgRequestException : Exception
    {
        public AgRequestException()
        { }

        public AgRequestException(string message) : base(message) { }

        public AgRequestException(string message, Exception innerException) : base(message, innerException) { }
    }
}
