using System;
namespace Xablu.WebApiClient.Client
{
    public class RefitClient : IRefitClient
    {
        private RefitService<IRefit> refitService;
        public RefitService<IRefit> RefitService => refitService;

        public RefitClient(string BaseApi)
        {
            refitService = new RefitService<IRefit>(BaseApi);
        }
    }
}