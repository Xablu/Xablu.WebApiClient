using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xablu.WebApiClient.Attributes;

namespace Xablu.WebApiClient.Services.GraphQL
{
    public class QueryRequest : BaseRequest
    {
        public QueryRequest(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentNullException(query, "Query specified is null, please pass a valid query.");
            }
            Query = query;
        }
    }

    public class QueryRequest<TResponseModel> : BaseRequest<TResponseModel>
        where TResponseModel : class
    {
        private int attributeNumber;

        public QueryRequest(params string[] optionalParameters)
        {
            OptionalParameters = optionalParameters;

            Query = BuildQuery();

            Debug.WriteLine($"GraphQL query string generated: {Query}");
        }

        public string[] OptionalParameters { get; set; }

        private string BuildQuery()
        {
            LoadProperties(typeof(TResponseModel));

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

        private void LoadProperties(Type type)
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
    }
}
