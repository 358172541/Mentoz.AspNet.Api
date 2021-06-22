using System;

namespace Mentoz.AspNet.Api
{
    public class TokenException : Exception
    {
        public TokenException(string message) : base(message) { }
    }
}