using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using GraphQL.Common.Request;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Xablu.WebApiClient.Services.GraphQL
{
    public class Request<T> : GraphQLRequest
    {
        private List<Dictionary<string, object>> ObjectPropsList = new List<Dictionary<string, object>>();

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
                Query = query;
            }
            GraphQLQuery = Query;
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


                propNames.Add(property.Name);
                propDict.Add(property.Name, null);

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
                var afterEndIndex = queryString.Count() + 1;
                queryString = queryString.Insert(0, "{").Insert(afterEndIndex, "}");
            }
            return queryString;
        }
    }
}
