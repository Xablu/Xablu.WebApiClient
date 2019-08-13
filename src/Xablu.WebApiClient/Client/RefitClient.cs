using System;
namespace Xablu.WebApiClient.Client
{
    public class RefitClient : IRefitClient
    {
        private RefitService<refit> refitService;
        public RefitService<refit> RefitService => refitService;

        public RefitClient(string BaseApi)
        {
            refitService = new RefitService<refit>(BaseApi);
        }
    }
}