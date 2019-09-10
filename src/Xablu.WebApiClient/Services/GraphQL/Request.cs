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
    {
        private List<Dictionary<string, object>> ObjectPropsList = new List<Dictionary<string, object>>();

        int attributeNumber;

        public Request(string query = "", object model = null, string[] optionalParameters = null)
        {
            if (model != null)
            {
                ResponseModel = (T)model;
                if (ResponseModel != null)
                {
                    var QueryString = GetQueryString(ResponseModel);

                    if (optionalParameters != null)
                    {
                        QueryString = FormatQuery(QueryString, optionalParameters);
                    }

                    GraphQLQuery = QueryString;
                    Query = GraphQLQuery;
                }

            }
            else
            {
                Query = query;
                GraphQLQuery = Query;
            }
        }

        private string GetQueryString(T obj)
        {
            //need help
            Type type = obj.GetType();
            Iterate(type);

            var result = CreateQuery();
            return result;
        }

        public string GraphQLQuery { get; set; }

        public T ResponseModel { get; set; }

        private void Iterate(Type type)
        {

            var props = new List<PropertyInfo>();
            var baseType = type;
            var propNames = new HashSet<object>();
            var propDict = new Dictionary<string, object>();


            while (baseType != typeof(object))
            {
                var ti = baseType.GetTypeInfo();

                var newProps = (
                    from p in ti.DeclaredProperties
                    where
                        !propNames.Contains(p.Name) &&
                        p.CanRead && p.CanWrite &&
                        (p.GetMethod != null) && (p.SetMethod != null) &&
                        (p.GetMethod.IsPublic && p.SetMethod.IsPublic) &&
                        (!p.GetMethod.IsStatic) && (!p.SetMethod.IsStatic)
                    select p).ToList();

                props.AddRange(newProps);
                baseType = ti.BaseType;
            }

            foreach (PropertyInfo property in props)
            {
                var propType = property.PropertyType;
                string propName = property.Name;


                var HasAttribute = Attribute.IsDefined(property, typeof(QueryParameter));

                if (HasAttribute)
                {
                    propNames.Add($"{property.Name}" + $"{{{attributeNumber.ToString()}}}");
                    propDict.Add($"{property.Name}" + $"{{{attributeNumber.ToString()}}}", null);

                    attributeNumber++;
                }
                else
                {
                    propNames.Add(property.Name);
                    propDict.Add(property.Name, null);
                }



                if (propType.IsClass)
                {
                    var test = propType.GetProperties();
                    var hasProperties = propType.GetProperties() != null && propType.GetProperties().Length > 0;
                    if (hasProperties)
                    {
                        Iterate(propType);
                    }
                }
            }
            if (propDict.Any())
            {
                ObjectPropsList.Add(propDict);
            }
        }

        private string CreateQuery()
        {
            string queryString = "";

            foreach (Dictionary<string, object> propDic in ObjectPropsList)
            {
                foreach (KeyValuePair<string, object> property in propDic.Reverse())
                {
                    if (queryString.Count() > 0)
                    {
                        queryString = queryString.Insert(0, property.Key.ToLower() + " ");
                    }
                    else
                    {
                        queryString = queryString.Insert(0, property.Key.ToLower());
                    }

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
