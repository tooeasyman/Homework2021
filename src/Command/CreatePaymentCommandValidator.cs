using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Rickie.Homework.ShowcaseApp.Persistence;

namespace Rickie.Homework.ShowcaseApp.Command
{
    /// <summary>
    ///     Validator for payment request
    /// </summary>
    public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
    {
        private readonly IUserRepositoryAsync _userRepository;

        /// <summary>
        ///     Ctor.
        /// </summary>
        /// <param name="userRepository"></param>
        public CreatePaymentCommandValidator(IUserRepositoryAsync userRepository)
        {
            _userRepository = userRepository;

            // No need to verify from user id as the paying user must have been authenticated

            // Check if receiving party is available
            RuleFor(p => p.PayTo)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MustAsync(IsUserExists).WithMessage("{PropertyName} not exists.");
        }

        /// <summary>
        ///     Check whether a user ID exists in the backend
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        private async Task<bool> IsUserExists(Guid userId, CancellationToken cancellationToken)
        {
            var userObject = (await _userRepository.FindByCondition(x => x.UserId.Equals(userId)).ConfigureAwait(false))
                .AsQueryable().FirstOrDefault();
            if (userObject != null) return true;
            return false;
        }
    }
}