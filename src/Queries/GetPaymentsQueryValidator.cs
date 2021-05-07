using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Rickie.Homework.ShowcaseApp.Persistence;

namespace Rickie.Homework.ShowcaseApp.Queries
{
    /// <summary>
    ///     Validator for <see cref="GetPaymentsQuery" />
    /// </summary>
    public class GetPaymentsQueryValidator : AbstractValidator<GetPaymentsQuery>
    {
        private readonly IUserRepositoryAsync _userRepository;

        public GetPaymentsQueryValidator(IUserRepositoryAsync userRepository)
        {
            _userRepository = userRepository;
            RuleFor(p => p.UserId)
                .NotEmpty().WithMessage("{PropertyName} is required.")
                .NotNull()
                .MustAsync(IsUserExists).WithMessage("{PropertyName} not exists.");
        }

        private async Task<bool> IsUserExists(Guid userId, CancellationToken cancellationToken)
        {
            var userObject = (await _userRepository.FindByCondition(x => x.UserId.Equals(userId)).ConfigureAwait(false))
                .AsQueryable().FirstOrDefault();
            if (userObject != null) return true;
            return false;
        }
    }
}