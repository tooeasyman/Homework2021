using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using Rickie.Homework.ShowcaseApp.Models;
using Rickie.Homework.ShowcaseApp.Persistence;
using Rickie.Homework.ShowcaseApp.Queries;
using Rickie.Homework.ShowcaseApp.Services;
using Xunit;

namespace Rickie.Homework.ShowcaseApp.Tests.UnitTests
{
    public class GetPaymentsQueryValidatorTest
    {
        [Fact]
        void ValidInputShouldPass()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetPaymentsQuery() {UserId = userId};
            var mock = new Mock<IUserRepositoryAsync>();
            mock.Setup(x => x.FindByCondition(y => y.UserId.Equals(userId)))
                .Returns(Task.FromResult(new List<User>() { new User() { UserId = userId } }.AsQueryable()));
            
            // Act and assert
            Assert.True(new GetPaymentsQueryValidator(mock.Object).Validate(query).IsValid);
        }


        [Fact]
        void InvalidInputsShouldFail()
        {
            // Arrange
            var queryWithoutUserId = new GetPaymentsQuery();
            var queryWithNonExistingUserId = new GetPaymentsQuery() {UserId = Guid.NewGuid()};
            var mock = new Mock<IUserRepositoryAsync>();
            mock.Setup(x => x.FindByCondition(y => true))
                .Returns(Task.FromResult(new List<User>().AsQueryable()));

            // Act and assert
            Assert.False(new GetPaymentsQueryValidator(mock.Object).Validate(queryWithoutUserId).IsValid);
            Assert.False(new GetPaymentsQueryValidator(mock.Object).Validate(queryWithNonExistingUserId).IsValid);
        }
    }
}
