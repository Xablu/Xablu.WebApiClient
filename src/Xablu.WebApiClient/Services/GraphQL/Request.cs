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
            Type type = obj.GetType(); 

            try
            { 
                Iterate(type);
            }
            catch (Exception ex)
            {

            }


            return null;
        }

        private List<HashSet<string>> ObjectProps = new List<HashSet<string>>();

        private void Iterate(Type type)
        {
            var props = new List<PropertyInfo>();
            var baseType = type;
            var propNames = new HashSet<string>();
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

            foreach (var property in props)
            {
                var propType = property.PropertyType;

                propNames.Add(property.Name);

                if (propType.IsClass)
                {
                    Iterate(propType);
                }
            }

            ObjectProps.Add(propNames);
        }


        public string GraphQLQuery { get; set; }

        public T ResponseModel { get; set; }
    }
}
