using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphQL.Common.Request;
using Xablu.WebApiClient.Attributes;

namespace Xablu.WebApiClient.Services.GraphQL
{
    public class Request<T> : GraphQLRequest
        where T : class
    {
        private List<Dictionary<string, object>> PropertyDictList = new List<Dictionary<string, object>>();

        int attributeNumber;

        public Request(string query = "", T model = default, string[] optionalParameters = null)
        {
            OptionalParameters = optionalParameters;
            Query = query;
            ResponseModel = model;

            CreateQuery(model);
        }

        public string GraphQLQuery { get; set; }

        public T ResponseModel { get; set; }

        public string[] OptionalParameters { get; set; }


        public void CreateQuery(T model)
        {
            if (model != null)
            {
                var queryString = GetQuery(ResponseModel);
                GraphQLQuery = queryString;
                Query = GraphQLQuery;
            }
            else
            {
                GraphQLQuery = Query;
            }
        }

        private string GetQuery(T obj)
        {
            GetProperties(obj.GetType());

            var unformattedQuery = QueryBuilder();

            var result = OptionalParameters != null ? FormatQuery(unformattedQuery, OptionalParameters) : unformattedQuery;

            return result;
        }

        private void GetProperties(Type type)
        {

            var propsList = new List<PropertyInfo>();
            var baseType = type;
            var propDict = new Dictionary<string, object>();

            while (baseType != typeof(object))
            {
                var typeInfo = baseType.GetTypeInfo();
                var newProps = typeInfo.DeclaredProperties.Where(p => !propDict.ContainsKey(p.Name) &&
                                                                p.CanRead && p.CanWrite &&
                                                                (p.GetMethod != null) && (p.SetMethod != null) &&
                                                                (p.GetMethod.IsPublic && p.SetMethod.IsPublic) &&
                                                                (!p.GetMethod.IsStatic) && (!p.SetMethod.IsStatic)).ToList();
                propsList.AddRange(newProps);
                baseType = typeInfo.BaseType;
            }

            foreach (PropertyInfo property in propsList)
            {
                var propType = property.PropertyType;
                string propName = property.Name;

                SetDictionary(property, propDict);

                if (propType.IsClass)
                {
                    var hasProperties = propType.GetProperties() != null && propType.GetProperties().Length > 0;
                    if (hasProperties)
                    {
                        GetProperties(propType);
                    }
                }
            }

            if (propDict.Any())
            {
                PropertyDictList.Add(propDict);
            }
        }

        private void SetDictionary(PropertyInfo property, Dictionary<string, object> propDict)
        {
            var hasAttribute = Attribute.IsDefined(property, typeof(QueryParameterAttribute)) || Attribute.IsDefined(property, typeof(NameOfItemAttribute));

            if (hasAttribute)
            {

                var queryParameter = (Attribute.GetCustomAttribute(property, typeof(QueryParameterAttribute)) as QueryParameterAttribute);

                if (string.IsNullOrEmpty(queryParameter?.ExclusiveWith))
                {
                    var propertyExists = propDict.ContainsKey(queryParameter.ExclusiveWith);

                    if (propertyExists)
                    {
                        propDict.Remove(queryParameter.ExclusiveWith);
                    }
                }

                var queryName = (Attribute.GetCustomAttribute(property, typeof(NameOfItemAttribute)) as NameOfItemAttribute)?.Values[0];
                var query = !string.IsNullOrEmpty(queryName) ? $"{property.Name}: {queryName}" : $"{property.Name}" + $"{{{attributeNumber.ToString()}}}";

                propDict.Add(query, null);
                attributeNumber++;
            }
            else
            {
                propDict.Add(property.Name, null);
            }
        }

        private string QueryBuilder()
        {
            string queryString = "";

            foreach (Dictionary<string, object> propertyDictonary in PropertyDictList)
            {
                foreach (KeyValuePair<string, object> property in propertyDictonary.Reverse())
                {
                    queryString = queryString.Any() ? queryString = queryString.Insert(0, ToLowerFirstChar(property.Key) + " ") : queryString.Insert(0, ToLowerFirstChar(property.Key));
                }
                queryString = "{{" + $"{queryString}" + "}}";
            }
            return queryString;
        }

        private string FormatQuery(string query, string[] optionalParameters)
        {
            var formattedString = (string.Format(query, optionalParameters));

            return formattedString;
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
