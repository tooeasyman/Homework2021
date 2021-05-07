using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Rickie.Homework.ShowcaseApp.Persistence;
using Rickie.Homework.ShowcaseApp.Queries;

namespace Rickie.Homework.ShowcaseApp.Command
{
    public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
    {
        private readonly IUserRepositoryAsync _userRepository;

        public CreatePaymentCommandValidator(IUserRepositoryAsync userRepository)
        {
            _userRepository = userRepository;
            RuleFor(p => p.PayTo)
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
