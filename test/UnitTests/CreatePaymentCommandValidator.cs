using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using Rickie.Homework.ShowcaseApp.Command;
using Rickie.Homework.ShowcaseApp.Models;
using Rickie.Homework.ShowcaseApp.Persistence;
using Rickie.Homework.ShowcaseApp.Queries;
using Rickie.Homework.ShowcaseApp.Services;
using Xunit;

namespace Rickie.Homework.ShowcaseApp.Tests.UnitTests
{
    public class CreatePaymentCommandValidatorTest
    {
        [Fact]
        void ValidInputShouldPass()
        {
            // Arrange
            var payTo = Guid.NewGuid();
            var command = new CreatePaymentCommand() {PayTo = payTo, Amount = 10};
            var mock = new Mock<IUserRepositoryAsync>();
            mock.Setup(x => x.FindByCondition(y => y.UserId.Equals(payTo)))
                .Returns(Task.FromResult(new List<User>() { new User() { UserId = payTo, UserName = "bbb"} }.AsQueryable()));
            
            // Act and assert
            Assert.True(new CreatePaymentCommandValidator(mock.Object).Validate(command).IsValid);
        }


        [Fact]
        void InvalidInputsShouldFail()
        {
            // Arrange
            var queryWithoutUserId = new CreatePaymentCommand();
            var queryWithNonExistingUserId = new CreatePaymentCommand() {UserId = Guid.NewGuid()};
            var mock = new Mock<IUserRepositoryAsync>();
            mock.Setup(x => x.FindByCondition(y => true))
                .Returns(Task.FromResult(new List<User>().AsQueryable()));

            // Act and assert
            Assert.False(new CreatePaymentCommandValidator(mock.Object).Validate(queryWithoutUserId).IsValid);
            Assert.False(new CreatePaymentCommandValidator(mock.Object).Validate(queryWithNonExistingUserId).IsValid);
        }
    }
}
