using System;
using System.Threading.Tasks;
using Moq;
using Xablu.WebApiClient.Client;
using Xablu.WebApiClient.Enums;
using Xablu.WebApiClient.Services.GraphQL;
using Xunit;

namespace Xablu.WebApiClient.Tests
{
    public class RefitTests
    {
        [Fact]
        public void CheckRetryCount()
        {
            // mock dependencies 
            var refitService = new Mock<IRefitService<IStarwarsApi>>();
            var graphqlService = new Mock<IGraphQLService>();
            var mockedAPI = new Mock<IStarwarsApi>();

            // setup dependency mocks
            refitService.SetupGet(s => s.GetByPriority(Priority.UserInitiated)).Returns(mockedAPI.Object);
            mockedAPI.Setup(api => api.Get()).Returns(Task.CompletedTask);

            var client = new WebApiClient<IStarwarsApi>(refitService.Object, graphqlService.Object);

            // complete call
            var call = client.Call(api => api.Get());

            // check if call has retried according to count
            if (call.Status != TaskStatus.Faulted)
            {
                // do something next to check retry counts...
            }
        }
    }
}
