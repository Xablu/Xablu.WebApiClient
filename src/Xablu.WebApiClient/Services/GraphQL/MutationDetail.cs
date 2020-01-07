using System;
namespace Xablu.WebApiClient.Services.GraphQL
{
    public class MutationDetail
    {
        public MutationDetail(string mutationName, object mutationVariable = null, params string[] mutationVariableNames)
        {
            MutationName = mutationName;
            MutationVariableNames = mutationVariableNames;
        }
        // Maybe change this to method name?
        public string MutationName { get; set; }
        public string[] MutationVariableNames { get; set; }

    }
}
