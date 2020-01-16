using System;
namespace Xablu.WebApiClient.Services.GraphQL
{
    public class MutationDetail
    {
        public MutationDetail(string mutationName, string mutationParameterInputName)
        {
            MutationName = mutationName;
            MutationParameterInputName = mutationParameterInputName;
        }
        // Maybe change this to method name?
        public string MutationName { get; set; }
        public string MutationParameterInputName { get; set; }

    }
}
