using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xablu.WebApiClient.Attributes;

namespace Xablu.WebApiClient.Services.GraphQL
{
    public class MutationRequest : BaseRequest
    {
        public MutationRequest(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentNullException(query, "Query specified is null, please pass a valid query.");
            }
            Query = query;
        }
    }

    public class MutationRequest<T> : BaseRequest
        where T : class
    {
        // Model fed should have a NameOfInputAttribute! todo throw exception if none is found!
        private List<List<PropertyDetail>> _propertyListList = new List<List<PropertyDetail>>();
        public MutationRequest(MutationDetail mutation, object variables)
        {
            Mutation = mutation;
            CreateMutationQuery();
        }


        public MutationDetail Mutation { get; set; }

        public string GraphQLMutationQuery { get; set; }
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
            if (string.IsNullOrEmpty(Query))
            {
                var queryString = GetQuery();
                GraphQLMutationQuery = queryString;
                Query = GraphQLMutationQuery;
            }
            else
            {
                GraphQLMutationQuery = Query;
            }
        }

        private string GetQuery()
        {
            var property = typeof(T);
            LoadProperties(property);
            CurateProperties();
            var query = QueryBuilder(property);
            return query;
        }

        protected virtual void LoadProperties(Type type)
        {
            var propsList = new List<PropertyInfo>();
            var baseType = type;
            var propList = new List<PropertyDetail>();

            while (baseType != typeof(object))
            {
                var typeInfo = baseType.GetTypeInfo();
                var newProps = typeInfo.DeclaredProperties.Where(p => !propList.Any(prop => prop.PropertyName.Equals(p.Name)) &&
                                                                p.CanRead && p.CanWrite &&
                                                                (p.GetMethod != null) && (p.SetMethod != null) &&
                                                                (p.GetMethod.IsPublic && p.SetMethod.IsPublic) &&
                                                                (!p.GetMethod.IsStatic) && (!p.SetMethod.IsStatic))
                                                          .ToList();

                propsList.AddRange(newProps);

                baseType = typeInfo.BaseType;
            }

            foreach (PropertyInfo property in propsList)
            {
                var propType = property.PropertyType;

                var propDetail = new PropertyDetail() { FieldName = property.Name, PropertyName = property.Name, ParentName = property.DeclaringType.Name };
                propList.Add(propDetail);

                if (propType.IsClass)
                {
                    var hasProperties = propType.GetProperties() != null && propType.GetProperties().Length > 0;
                    if (hasProperties)
                    {
                        LoadProperties(propType);
                    }
                }
            }

            if (propList.Any())
            {
                _propertyListList.Add(propList);
            }
        }

        protected virtual void CurateProperties()
        {
            var parentTypeToExclude = typeof(List<>);
            foreach (List<PropertyDetail> propertyList in _propertyListList.Where(list => list.Any()))
            {
                propertyList.RemoveAll(p => p.ParentName == parentTypeToExclude.Name);
            }
        }

        private string QueryBuilder(Type property)
        {
            //todo also make it possible to not use the attribute but a variable inside the mutationdetail class // also add null check
            var variableInputName = (Attribute.GetCustomAttribute(property, typeof(VariableInputAttribute)) as VariableInputAttribute)?.ModelInputName;
            var variableString = CreateVariableString();
            var methodString = CreateInputString(Mutation, variableInputName);
            var queryString = $"mutation{methodString}{variableString}";
            return queryString;
        }

        private string CreateVariableString()
        {
            string queryString = "";

            foreach (List<PropertyDetail> propertyList in _propertyListList.Where(list => list.Any()))
            {
                propertyList.Reverse();
                foreach (PropertyDetail property in propertyList)
                {
                    queryString = queryString.Any() ? queryString.Insert(0, ToLowerFirstChar(property?.FieldName) + " ") : queryString.Insert(0, ToLowerFirstChar(property?.FieldName));
                }
                queryString = "{" + $"{queryString}" + "}}";
            }
            return queryString;
        }

        private string CreateInputString(MutationDetail mutationDetail, string variableInputName)
        {
            string inputString = "";
            if (mutationDetail != null && !string.IsNullOrEmpty(variableInputName))
            {
                inputString = $"($MutationParameterInputName: {variableInputName}!)" + $"{{{mutationDetail.MutationName}(${mutationDetail.MutationParameterName}: $MutationParameterInputName)";
            }
            {
                return inputString;
            }
            // ($changeUserStatusInputModel: ChangeUserStatusInput!) { changeUserStatus(input: $changeUserStatusInputModel)


            // var mutationQuery = @" mutation ($changeUserStatusInputModel: ChangeUserStatusInput!) { changeUserStatus(input: $changeUserStatusInputModel){ clientMutationId, status { message }}}";
        }

        private string ToLowerFirstChar(string input)
        {
            string newString = input;
            if (!string.IsNullOrEmpty(newString) && char.IsUpper(newString[0]))
                newString = char.ToLower(newString[0]) + newString.Substring(1);
            return newString;
        }
    }
}
