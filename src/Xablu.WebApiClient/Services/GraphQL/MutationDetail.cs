using System;
namespace Xablu.WebApiClient.Services.GraphQL
{
    public class MutationDetail
    {
        public MutationDetail(string mutationName, string mutationParameterName)
        {
            MutationName = mutationName;
            MutationParameterName = mutationParameterName;
        }
        // Maybe change this to method name?
        public string MutationName { get; set; }
        public string MutationParameterName { get; set; }

    }
}
