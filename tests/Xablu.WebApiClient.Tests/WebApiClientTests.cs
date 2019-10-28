using System;
using System.Threading.Tasks;
using Moq;
using Refit;
using Xablu.WebApiClient.Client;
using Xablu.WebApiClient.Enums;
using Xablu.WebApiClient.Services.GraphQL;
using Xunit;

namespace Xablu.WebApiClient.Tests
{
    public interface IStarwarsApi
    {
        [Get("/starships")]
        Task Get();
    }

    public class WebApiClientTests
    {
        [Fact]
        public void EnsureUserInitiatedIsSelectedByDefault()
        {
            // mock dependencies 
            var refitService = new Mock<IRefitService<IStarwarsApi>>();
            var graphqlService = new Mock<IGraphQLService>();
            var mockedAPI = new Mock<IStarwarsApi>();

            // setup dependency mocks
            refitService.SetupGet(s => s.GetByPriority(Priority.UserInitiated)).Returns(mockedAPI.Object);
            mockedAPI.Setup(api => api.Get()).Returns(Task.CompletedTask);

            var client = new WebApiClient<IStarwarsApi>(refitService.Object, graphqlService.Object);

            // execute sentence to test
            client.Call(api => api.Get());

            // make sure UserInitiated was called
            refitService.VerifyGet(s => s.GetByPriority(Priority.UserInitiated));
        }

        [Fact]
        public void EnsureOperationIsCalledInTheRefitAPI()
        {
            // mock dependencies 
            var refitService = new Mock<IRefitService<IStarwarsApi>>();
            var graphqlService = new Mock<IGraphQLService>();
            var mockedAPI = new Mock<IStarwarsApi>();

            // setup dependency mocks
            refitService.SetupGet(s => s.GetByPriority(Priority.UserInitiated)).Returns(mockedAPI.Object);
            mockedAPI.Setup(api => api.Get()).Returns(Task.CompletedTask);

            var client = new WebApiClient<IStarwarsApi>(refitService.Object, graphqlService.Object);

            // execute sentence to test
            client.Call(api => api.Get());

            // make sure IStarwarsApi.Get was called
            mockedAPI.Verify(api => api.Get(), Times.Once);
        }
    }
}
