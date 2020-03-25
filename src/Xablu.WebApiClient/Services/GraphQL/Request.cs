using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xablu.WebApiClient.Attributes;

namespace Xablu.WebApiClient.Services.GraphQL
{
    public class Request : BaseRequest
    {
        public Request(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentNullException(query, "Query specified is null, please pass a valid query.");
            }
            Query = query;
        }
    }

    public class Request<T> : BaseRequest
        where T : class
    {
        private List<List<PropertyDetail>> _propertyListList = new List<List<PropertyDetail>>();
        private int attributeNumber;

        public Request(params string[] optionalParameters)
        {
            OptionalParameters = optionalParameters;
            CreateQuery();
        }

        public string GraphQLQuery { get; set; }

        public string[] OptionalParameters { get; set; }

        private void CreateQuery()
        {
            if (string.IsNullOrEmpty(Query))
            {
                var queryString = GetQuery();
                GraphQLQuery = queryString;
                Query = GraphQLQuery;

                Debug.WriteLine($"GraphQL query string generated: {Query}");
            }
            else
            {
                GraphQLQuery = Query;
            }
        }

        protected virtual string GetQuery()
        {
            LoadProperties(typeof(T));

            CurateProperties();

            var unformattedQuery = QueryBuilder();

            var result = OptionalParameters != null ? FormatQuery(unformattedQuery, OptionalParameters) : unformattedQuery;

            if (string.IsNullOrEmpty(result))
            {
                var errorMessage = string.IsNullOrEmpty(unformattedQuery) ? "No valid query. Something went wrong with building the query." : "No valid query. Something went wrong when formatting the query";
                throw new RequestException(errorMessage);
            }

            return result;
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

                PopulatePropertyDetailsByProperty(property, propList, propDetail);

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

        private void PopulatePropertyDetailsByProperty(PropertyInfo property, List<PropertyDetail> propertyList, PropertyDetail propertyDetail)
        {
            var queryName = Attribute.GetCustomAttribute(property, typeof(NameOfFieldAttribute)) as NameOfFieldAttribute;
            string query = property.Name;

            if (OptionalParameters != null && attributeNumber < OptionalParameters.Count())
            {
                query = !string.IsNullOrEmpty(queryName?.NameOfField) ? $"{property?.Name}" + $"{{{attributeNumber.ToString()}}}: {queryName?.NameOfField}" : $"{property?.Name}" + $"{{{attributeNumber.ToString()}}}";
                attributeNumber++;
            }

            if (queryName != null)
            {
                if (!string.IsNullOrEmpty(queryName.NameOfField))
                {
                    query = $"{property.Name}: {queryName.NameOfField}";
                }
            }

            propertyDetail.FieldName = query;
            propertyList.Add(propertyDetail);
        }

        private string QueryBuilder()
        {
            string queryString = "";

            foreach (List<PropertyDetail> propertyList in _propertyListList.Where(list => list.Any()))
            {
                propertyList.Reverse();
                foreach (PropertyDetail property in propertyList)
                {
                    queryString = queryString.Any() ? queryString.Insert(0, ToLowerFirstChar(property?.FieldName) + " ") : queryString.Insert(0, ToLowerFirstChar(property?.FieldName));
                }
                queryString = "{{" + $"{queryString}" + "}}";
            }
            return queryString;
        }

        private string FormatQuery(string query, string[] optionalParameters)
        {
            var formattedString = string.Format(query, optionalParameters);

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
