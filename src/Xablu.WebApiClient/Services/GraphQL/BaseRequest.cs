using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL;

namespace Xablu.WebApiClient.Services.GraphQL
{
    public class BaseRequest : GraphQLRequest
    { 
    }

    public class BaseRequest<TResponseModel> : GraphQLRequest
        where TResponseModel : class
    {
        protected List<List<PropertyDetail>> _propertyListList = new List<List<PropertyDetail>>();
         
        protected virtual void CurateProperties()
        {
            var parentTypeToExclude = typeof(List<>);
            foreach (List<PropertyDetail> propertyList in _propertyListList.Where(list => list.Any()))
            {
                propertyList.RemoveAll(p => p.ParentName == parentTypeToExclude.Name);
            }
        }

        protected virtual string ToLowerFirstChar(string input)
        {
            string newString = input;
            if (!string.IsNullOrEmpty(newString) && char.IsUpper(newString[0]))
                newString = char.ToLower(newString[0]) + newString.Substring(1);
            return newString;
        }
    }
}
