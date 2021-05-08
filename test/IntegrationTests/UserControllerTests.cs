using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Newtonsoft.Json.Linq;
using Rickie.Homework.ShowcaseApp.Controllers;
using Rickie.Homework.ShowcaseApp.Models;
using Rickie.Homework.ShowcaseApp.Persistence;
using Xunit;

namespace Rickie.Homework.ShowcaseApp.Tests.IntegrationTests
{
    public class UserControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public UserControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }


        [Fact]
        public async Task RequestsWithoutJWTShouldBeRejected()
        {
            // Arrange
            var baseUri = "/api/user/";
            var client = _factory.CreateClient();
            var body = new StringContent("{}", Encoding.UTF8, "application/json");

            // Act and assert
            Assert.Equal(HttpStatusCode.Unauthorized,
                (await client.PostAsync(baseUri + "getallusers", body)).StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized,
                (await client.PostAsync(baseUri + "createpayment", body)).StatusCode);
            Assert.Equal(HttpStatusCode.Unauthorized,
                (await client.PostAsync(baseUri + "getpayments", body)).StatusCode);
        }

        [Fact]
        public async Task TestGetAllUsers()
        {
            // Arrange
            var uri = "/api/user/getallusers";
            var body = new StringContent("{}", Encoding.UTF8, "application/json");
            var expectedUsers = new List<User>
            {
                new User {UserId = Guid.NewGuid()},
                new User {UserId = Guid.NewGuid()},
                new User {UserId = Guid.NewGuid()}
            };
            var mock = new Mock<IUserRepositoryAsync>();
            mock.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(expectedUsers.AsQueryable()));
            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(mock.Object);
                    services.AddSingleton<IAuthorizationHandler>(new AllowAnonymous());
                });
            }).CreateClient();


            // Act
            var response = await client.PostAsync(uri, body);
            var contents = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(contents);

            // Assert
            Assert.Equal(expectedUsers.Count, json["data"].Count());
        }


        [Fact]
        public async Task TestGetPayments()
        {
            // Arrange
            var uri = "/api/user/getpayments";
            var user = new User { UserId = Guid.NewGuid(), UserName = "Jake" };
            var body = new StringContent("{\"UserId\":\"" +user.UserId + "\"}", Encoding.UTF8, "application/json");
            var payments = new List<Payment>
            {
                new Payment {UserId = user.UserId, Amount = 10, PayTo = Guid.NewGuid(), Date = DateTime.Now.AddDays(-10)},
                new Payment {UserId = user.UserId, Amount = 20, PayTo = Guid.NewGuid(), Date = DateTime.Now.AddDays(-6)},
                new Payment {UserId = user.UserId, Amount = 30, PayTo = Guid.NewGuid(), Date = DateTime.Now.AddDays(-1)}
            };

            var userRepositoryMock = new Mock<IUserRepositoryAsync>();
            userRepositoryMock.Setup(x => x.FindByCondition(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(Task.FromResult(new List<User> {user}.AsQueryable()));

            var userBalanceMock = new Mock<IUserBalanceRepositoryAsync>();
            userBalanceMock.Setup(x => x.FindByCondition(It.IsAny<Expression<Func<UserBalance, bool>>>())).Returns(
                Task.FromResult(new List<UserBalance> {new UserBalance {UserId = user.UserId, Balance = 888}}
                    .AsQueryable()));

            var paymentsRepositoryMock = new Mock<IPaymentRepositoryAsync>();
            paymentsRepositoryMock.Setup(x => x.FindByCondition(It.IsAny<Expression<Func<Payment, bool>>>())).Returns(
                Task.FromResult(payments.AsQueryable()));

            var client = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddSingleton(userRepositoryMock.Object);
                    services.AddSingleton(userBalanceMock.Object);
                    services.AddSingleton(paymentsRepositoryMock.Object);
                    services.AddSingleton<IAuthorizationHandler>(new AllowAnonymous());
                });
            }).CreateClient();
            
            // Act
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + JWTGenerator.GenerateJWToken(user));
            var response = await client.PostAsync(uri, body);
            var contents = await response.Content.ReadAsStringAsync();
            var json = JObject.Parse(contents);

            // Assert
            Assert.Equal(payments.Count, json["data"]["payments"].Count());
            Assert.Equal("888", json["data"]["balance"]);
            
            // The first payment returned should be the latest with amount 30.
            Assert.Equal("30", json["data"]["payments"][0]["amount"]);
        }

    }
}