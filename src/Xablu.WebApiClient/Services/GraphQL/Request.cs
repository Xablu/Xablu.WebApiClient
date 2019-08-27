using System;
using System.Linq;
using System.Web;
using GraphQL.Common.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Xablu.WebApiClient.Services.GraphQL
{
    public class Request<T> : GraphQLRequest
    {

        public Request(string query = "", object model = null)
        {
            if (model != null)
            {
                ResponseModel = (T)model;
                if (ResponseModel != null)
                {

                    GraphQLQuery = GetQueryString(ResponseModel);
                    Query = GraphQLQuery;
                }

            }
            else
            {
                base.Query = query;
            }
        }

        private string GetQueryString(T obj)
        {
            //need help
            return null;
        }

        public string GraphQLQuery { get; set; }

        public T ResponseModel { get; set; }

    }


}
