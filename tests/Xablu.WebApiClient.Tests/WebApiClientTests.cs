using System;
using System.Threading.Tasks;
using Moq;
using Refit;
using Xablu.WebApiClient.Client;
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
            var service = new Mock<IRefitService<IStarwarsApi>>();
            var mockedAPI = new Mock<IStarwarsApi>();

            // setup dependency mocks
            service.SetupGet(s => s.UserInitiated).Returns(mockedAPI.Object);
            mockedAPI.Setup(api => api.Get()).Returns(Task.CompletedTask);

            var client = new WebApiClient<IStarwarsApi>(service.Object);

            // execute sentence to test
            client.Call(api => api.Get());

            // make sure UserInitiated was called
            service.VerifyGet(s => s.UserInitiated);
        }

        [Fact]
        public void EnsureOperationIsCalledInTheRefitAPI()
        {
            // mock dependencies 
            var service = new Mock<IRefitService<IStarwarsApi>>();
            var mockedAPI = new Mock<IStarwarsApi>();

            // setup dependency mocks
            service.SetupGet(s => s.UserInitiated).Returns(mockedAPI.Object);
            mockedAPI.Setup(api => api.Get()).Returns(Task.CompletedTask).Verifiable();

            var client = new WebApiClient<IStarwarsApi>(service.Object);

            // execute sentence to test
            client.Call(api => api.Get());

            // make sure IStarwarsApi.Get was called
            mockedAPI.Verify(api => api.Get(), Times.Once);
        }
    }
}
