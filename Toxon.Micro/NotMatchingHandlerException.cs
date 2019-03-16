using System;

namespace Toxon.Micro
{
    public class NotMatchingHandlerException : Exception
    {
        public IRequest Request { get; }

        public NotMatchingHandlerException(IRequest request)
            : base("No matching handler was found for the request")
        {
            Request = request;
        }
    }
}