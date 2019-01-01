using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace FluentValidation.Extensions
{
    /// <summary>
    /// FluentValidation Extension Methods
    /// </summary>
    public static class ValidationExtensions
    {
        /// <summary>
        /// Validates an entity using Fluent validator
        /// </summary>
        /// <typeparam name="T">Entity type</typeparam>
        /// <param name="validator">Entity validator</param>
        /// <param name="entity">Entity to validate</param>
        /// <returns>Dictionary of errors keyed by the entity property name</returns>
        public static IDictionary<string, IEnumerable<string>> ValidateEntity<T>(
            this AbstractValidator<T> validator,
            T entity)
            where T : class
        {
            if (validator == null) throw new ArgumentNullException(nameof(validator));
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var result = validator.Validate(entity);
            if (result == null || result.Errors == null || !result.Errors.Any())
            {
                return new Dictionary<string, IEnumerable<string>>();
            }

            return result.Errors.GetErrors();
        }

        /// <summary>
        /// Get errors from validation failures
        /// </summary>
        /// <param name="failures">Validation failures</param>
        /// <returns>Dictionary of errors keyed by the entity property name</returns>
        public static IDictionary<string, IEnumerable<string>> GetErrors(this IList<ValidationFailure> failures)
        {
            var errors = new Dictionary<string, IEnumerable<string>>();

            foreach (var item in failures)
            {
                var propertyErrors = default(List<string>);

                if (errors.ContainsKey(item.PropertyName))
                {
                    propertyErrors = errors[item.PropertyName].ToList();
                }
                else
                {
                    propertyErrors = new List<string>();
                }

                propertyErrors.Add(item.ErrorMessage);

                if (errors.ContainsKey(item.PropertyName))
                {
                    errors[item.PropertyName] = propertyErrors;
                }
                else
                {
                    errors.Add(item.PropertyName, propertyErrors);
                }
            }

            return errors;
        }

        /// <summary>
        /// Gets a multi-line error message from a dictionary of error messages
        /// </summary>
        /// <param name="errors">Errors to concatenate</param>
        /// <returns>A multi-line error message</returns>
        public static string GetMultiLineErrorMessage(this IDictionary<string, IEnumerable<string>> errors)
        {
            if (errors == null) throw new ArgumentNullException(nameof(errors));
            if (!errors.Any()) throw new ArgumentException("No errors to use to generate message");

            return errors
                .SelectMany(i => i.Value)
                .Aggregate((current, next) => current + Environment.NewLine + next);
        }
    }
}
