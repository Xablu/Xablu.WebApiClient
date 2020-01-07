using System;
namespace Xablu.WebApiClient.Services.GraphQL
{
    public class MutationRequest<T> : BaseRequest
        where T : class
    {
        // Model fed should have a NameOfInputAttribute! todo throw exception if none is found!
        public MutationRequest(MutationDetail mutation)
        {
            Mutation = mutation;
            CreateMutationQuery();

            var variables = new
            {
                changeUserStatusInputModel = new object()
                {
                    //       ClientMutationId = "101010101",
                    //       Message = "The mutation has succeeded"
                }
            };
        }


        public MutationDetail Mutation { get; set; }

        // so the query should have the following inputs:
        // "mutation ($"{InputAttributeName}: {reviewInput}"
        //        "mutation ($InputAttributeName: reviewInput!) { Mutation[0](review: ${InputAttributeName}) { id review } }"

        // -> solution probably  // "mutation ($VariableInputAttribute: {VariableInputName + "Input!"}) { {Mutation.MutationName}({Mutation.MutationVariableNames}: $VariableInputAttribute) { id review } }"




        // "mutation ($review: reviewInput!) { createReview(review: $review) { id review } }"
        //@" mutation ($changeUserStatusInputModel: ChangeUserStatusInput!) { changeUserStatus(input: $changeUserStatusInputModel){ clientMutationId, status { message }}}";

        //

        // mutation (
        // too difficult just do the following;
        // 
        private void CreateMutationQuery()
        {
            //if (string.IsNullOrEmpty(Query))
            //{
            //    var queryString = GetQuery();
            //    GraphQLQuery = queryString;
            //    Query = GraphQLQuery;
            //}
            //else
            //{
            //    GraphQLQuery = Query;
            //}
        }

        private string GetQuery()
        {
            //LoadProperties(typeof(T));

            //CurateProperties();

            //var unformattedQuery = QueryBuilder();

            //var result = OptionalParameters != null ? FormatQuery(unformattedQuery, OptionalParameters) : unformattedQuery;

            //if (string.IsNullOrEmpty(result))
            //{
            //    var errorMessage = string.IsNullOrEmpty(unformattedQuery) ? "No valid query. Something went wrong with building the query." : "No valid query. Something went wrong when formatting the query";
            //    throw new RequestException(errorMessage);
            //}

            return null; //result;
        }


    }
}
