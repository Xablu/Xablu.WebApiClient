using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GraphQL.Common.Request;
using Newtonsoft.Json.Linq;
using Xablu.WebApiClient.Attributes;
using Xablu.WebApiClient.Enums;

namespace Xablu.WebApiClient.Services.GraphQL
{
    public class Request<T> : GraphQLRequest
        where T : class
    {
        private List<Dictionary<string, object>> _propertyDictList = new List<Dictionary<string, object>>();
        private List<string> _exclusiveWithValues = new List<string>();
        private GraphQLService _service;


        private int attributeNumber;

        public Request(string query = "", string[] optionalParameters = null)
        {
            OptionalParameters = optionalParameters;
            Query = query; 

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

        public async Task<List<string>> GetMutlipleResults(string[] values, GraphQLService service, Priority priority)
        {
            var resultList = new List<string>();
            if (service != null)
            {
                _service = service;

                foreach (var valueName in values)
                {

                    var result = await _service.GetByPriority(priority).SendQueryAsync(GraphQLQuery).ConfigureAwait(false);
                    if (result.Data != null)
                    {

                        string resultValue;
                        var jObject = result.Data as JObject;
                        if (jObject != null)
                        {
                            resultValue = GetJValue(jObject, valueName);
                            if (!string.IsNullOrEmpty(resultValue))
                            {
                                resultList.Add(resultValue);
                            }
                        }
                    }
                }
            }
            return resultList;
        }

        private string GetJValue(JObject jObject, string valueName)
        {
            string resultValue = "";

            if (string.IsNullOrEmpty(resultValue))
            {
                foreach (var item in jObject.Properties())
                {
                    if (string.Equals(item.Name.ToLower(), valueName.ToLower()))
                    {
                        resultValue = item.HasValues ? item.Value.ToString() : null;
                        break;
                    }

                    if (string.IsNullOrEmpty(resultValue))
                    {
                        var hasChildren = item.Children().Any();
                        if (hasChildren)
                        {
                            foreach (var child in item.Children())
                            {
                                var newObject = child as JObject;
                                if (newObject != null && string.IsNullOrEmpty(resultValue))
                                {
                                    resultValue = GetJValue(newObject, valueName);
                                }
                            }
                        }
                    }
                }
            }
            return resultValue?.ToString();
        }

        private string GetQuery()
        {
            GetProperties(typeof(T));

            var unformattedQuery = QueryBuilder();

            var result = OptionalParameters != null ? FormatQuery(unformattedQuery, OptionalParameters) : unformattedQuery;

            return result;
        }

        private void GetProperties(Type type, object parentType = null)
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
                var parent = parentType ?? property;
                string propName = property.Name;

                SetDictionary(property, propDict, parent);

                if (propType.IsClass)
                {
                    var hasProperties = propType.GetProperties() != null && propType.GetProperties().Length > 0;
                    if (hasProperties)
                    {
                        GetProperties(propType, parent);
                    }
                }
            }

            if (propDict.Any())
            {
                _propertyDictList.Add(propDict);
            }

            RemoveExcluded();

        }

        private void RemoveExcluded()
        {
            if (_exclusiveWithValues.Any())
            {
                for (var i = _propertyDictList.Count - 1; i >= 0; i--)
                {
                    for (var p = _propertyDictList[i].Count - 1; p >= 0; p--)
                    {
                        var valuePair = _propertyDictList[i].ElementAt(p);
                        var valueParentType = valuePair.Value as PropertyInfo;
                        if (valueParentType != null)
                        {
                            var test = _exclusiveWithValues.Any(s => s.Equals(valueParentType.Name, StringComparison.OrdinalIgnoreCase));
                            if (test)
                            {
                                _propertyDictList[i].Remove(valuePair.Key);
                            }
                        }
                    }
                }
                _propertyDictList.RemoveAll(p => p.Count == 0);
                // this does not work sadly
                //_propertyDictList.RemoveAll(dict =>
                //{
                //    bool result = false;
                //    foreach (var item in dict.Values)
                //    {
                //        var parentType = item as PropertyInfo;
                //        if (parentType != null)
                //        {
                //            return result = _exclusiveWithValues.Any(c => c.Equals(parentType.Name, StringComparison.OrdinalIgnoreCase));
                //        }
                //    }
                //    return result;
                //});
            }
        }

        private void SetDictionary(PropertyInfo property, Dictionary<string, object> propDict, object parentType)
        {
            var hasAttribute = Attribute.IsDefined(property, typeof(QueryParameterAttribute)) || Attribute.IsDefined(property, typeof(NameOfItemAttribute));

            if (hasAttribute)
            {

                var queryParameter = (Attribute.GetCustomAttribute(property, typeof(QueryParameterAttribute)) as QueryParameterAttribute);

                if (!string.IsNullOrEmpty(queryParameter?.ExclusiveWith))
                {
                    _exclusiveWithValues.Add(queryParameter.ExclusiveWith);
                }

                var queryName = (Attribute.GetCustomAttribute(property, typeof(NameOfItemAttribute)) as NameOfItemAttribute)?.Values[0];
                var query = !string.IsNullOrEmpty(queryName) ? $"{property.Name}: {queryName}" : $"{property.Name}" + $"{{{attributeNumber.ToString()}}}";

                propDict.Add(query, parentType);
                attributeNumber++;
            }
            else
            {
                propDict.Add(property.Name, parentType);
            }
        }

        private string QueryBuilder()
        {
            string queryString = "";

            foreach (Dictionary<string, object> propertyDictonary in _propertyDictList)
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
