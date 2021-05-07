using System;
using System.Collections.Generic;
using FluentValidation.Results;

namespace Rickie.Homework.ShowcaseApp.CustomExceptions
{
    /// <summary>
    ///     Custom exception that represents business logic validation errors
    /// </summary>
    public class ValidationException : Exception
    {
        public ValidationException() : base("One or more validation failures have occurred.")
        {
            Errors = new List<ErrorModel>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures)
            : this()
        {
            foreach (var failure in failures)
                Errors.Add(new ErrorModel
                {
                    PropertyName = failure.PropertyName,
                    ErrorMessage = failure.ErrorMessage
                });
        }

        public List<ErrorModel> Errors { get; }
    }

    public class ErrorModel
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }
}