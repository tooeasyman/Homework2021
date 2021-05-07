using System.Collections.Generic;
using Rickie.Homework.ShowcaseApp.CustomExceptions;

namespace Rickie.Homework.ShowcaseApp.Models
{
    /// <summary>
    ///     Represents top level JSON object returned by APIs
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        ///     Default constructor
        /// </summary>
        public ApiResponse()
        {
        }

        public ApiResponse(T data, string message = null)
        {
            Succeeded = true;
            Message = message;
            Data = data;
        }

        public ApiResponse(string message)
        {
            Succeeded = false;
            Message = message;
        }

        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<ErrorModel> Errors { get; set; }
        public T Data { get; set; }
    }
}