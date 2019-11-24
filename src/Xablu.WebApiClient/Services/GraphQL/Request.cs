using System;
using System.Collections.Generic;
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

        public void CreateQuery()
        {
            if (string.IsNullOrEmpty(Query))
            {
                var queryString = GetQuery();
                GraphQLQuery = queryString;
                Query = GraphQLQuery;
            }
            else
            {
                GraphQLQuery = Query;
            }
        }


        //private string GetJValue(JObject jObject, string valueName)
        //{
        //    string resultValue = "";

        //    if (string.IsNullOrEmpty(resultValue))
        //    {
        //        foreach (var item in jObject.Properties())
        //        {
        //            if (string.Equals(item.Name.ToLower(), valueName.ToLower()))
        //            {
        //                resultValue = item.HasValues ? item.Value.ToString() : null;
        //                break;
        //            }

        //            if (string.IsNullOrEmpty(resultValue))
        //            {
        //                var hasChildren = item.Children().Any();
        //                if (hasChildren)
        //                {
        //                    foreach (var child in item.Children())
        //                    {
        //                        var newObject = child as JObject;
        //                        if (newObject != null && string.IsNullOrEmpty(resultValue))
        //                        {
        //                            resultValue = GetJValue(newObject, valueName);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return resultValue?.ToString();
        //}

        private string GetQuery()
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

        protected virtual void CurateProperties()
        {
            var parentTypeToExclude = typeof(List<>);
            foreach (List<PropertyDetail> propertyList in _propertyListList.Where(list => list.Any()))
            {
                propertyList.RemoveAll(p => p.ParentName == parentTypeToExclude.Name);
            }
        }

        //private void RemoveExcluded()
        //{
        //    if (_exclusiveWithValues.Any())
        //    {
        //        for (var i = _propertyDictList.Count - 1; i >= 0; i--)
        //        {
        //            for (var p = _propertyDictList[i].Count - 1; p >= 0; p--)
        //            {
        //                var valuePair = _propertyDictList[i].ElementAt(p);
        //                var tag = valuePair.Value?.PropertyName;
        //                var parentName = valuePair.Value?.ParentName;
        //                if (!string.IsNullOrEmpty(tag))
        //                {
        //                    var result = _exclusiveWithValues.Any(s => s.Equals(tag, StringComparison.OrdinalIgnoreCase) || s.Equals(parentName, StringComparison.OrdinalIgnoreCase));
        //                    if (result)
        //                    {
        //                        _propertyDictList[i].Remove(valuePair.Key);
        //                    }
        //                }
        //            }
        //        }
        //        _propertyDictList.RemoveAll(p => p.Count == 0);

        //    }
        //}

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
