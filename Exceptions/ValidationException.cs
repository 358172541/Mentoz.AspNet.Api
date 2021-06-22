using System;

namespace Mentoz.AspNet.Api
{
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }
}