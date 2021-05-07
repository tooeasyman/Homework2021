using System;
using System.Globalization;

namespace Rickie.Homework.ShowcaseApp.CustomExceptions
{
    /// <summary>
    ///     Represents errors in Web API calls
    /// </summary>
    public class ApiException : Exception
    {
        public ApiException()
        {
        }

        public ApiException(string message) : base(message)
        {
        }

        public ApiException(string message, params object[] args)
            : base(string.Format(CultureInfo.InvariantCulture, message, args))
        {
        }
    }
}