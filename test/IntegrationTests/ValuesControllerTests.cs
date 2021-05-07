using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Rickie.Homework.ShowcaseApp.Services;
using Xunit;

namespace Rickie.Homework.ShowcaseApp.Tests.IntegrationTests
{
    public class ValuesControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {

        public ValuesControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        private readonly WebApplicationFactory<Startup> _factory;
       

        [Fact]
        public async Task TestSomeMoo()
        {
            // // Arrange
            // var mock = new Mock<IValuesService>();
            // mock.Setup(t => t.GetValues()).Returns(new List<string>() {"value1"});
            // var service = mock.Object;
            //
            //
            // using var client = _factory.WithWebHostBuilder(builder =>
            // {
            //     builder.ConfigureTestServices(services =>
            //     {
            //         services.AddSingleton<IValuesService>(service);
            //     });
            // }).CreateClient();
            //
            // var index = await client.GetStringAsync("/api/values");
            // Assert.Equal("[\"value1\"]", index);
        }

        // private IValuesService _service;


    }
}
