using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using GraphQL.Common.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

                var hasAttribute = Attribute.IsDefined(property, typeof(QueryParameter));

                if (hasAttribute)
                {
                    var IsExclusive = Attribute.GetCustomAttribute(property, typeof(QueryParameter)) != null;
                    if (!IsExclusive)
                    {
                        propDict.Add($"{property.Name}" + $"{{{attributeNumber.ToString()}}}", null);
                        attributeNumber++;
                    }
                }
                else
                {
                    propDict.Add(property.Name, null);
                }

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

        private string QueryBuilder()
        {
            string queryString = "";

            foreach (Dictionary<string, object> propertyDictonary in PropertyDictList)
            {
                foreach (KeyValuePair<string, object> property in propertyDictonary.Reverse())
                {
                    queryString = queryString.Any() ? queryString = queryString.Insert(0, property.Key.ToLower() + " ") : queryString.Insert(0, property.Key.ToLower());
                }
                queryString = "{{" + $"{queryString}" + "}}";
            }
            return queryString;
        }

        private string FormatQuery(string query, string[] optionalParameters)
        {
            var formattedString = (string.Format(query, optionalParameters)).ToLower();

            return formattedString.ToLower();
        }
    }
}
