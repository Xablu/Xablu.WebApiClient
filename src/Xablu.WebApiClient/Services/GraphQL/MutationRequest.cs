using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Xablu.WebApiClient.Attributes;

namespace Xablu.WebApiClient.Services.GraphQL
{
    public class MutationRequest : BaseRequest
    {
        public MutationRequest(string query, object variables)
        {
            if (string.IsNullOrEmpty(query))
            {
                throw new ArgumentNullException(query, "Query specified is null, please pass a valid query.");
            }
            Query = query;
            Variables = new { variables };
        }
    }

    public class MutationRequest<TResponseModel> : BaseRequest<TResponseModel>
        where TResponseModel : class
    {
        private Type _requestObjectType;

        public MutationRequest(string mutationName, string mutationParameterName, object variable)
        {
            _requestObjectType = variable.GetType();

            MutationName = mutationName;
            MutationParameterName = mutationParameterName;

            Variables = new { variable };

            Query = BuildQuery();

            Debug.WriteLine($"GraphQL mutation query string generated: {Query}");
        }

        public string MutationName { get; set; }
        public string MutationParameterName { get; set; } 
         
        private string BuildQuery()
        {
            LoadProperties(typeof(TResponseModel));

            _propertyListList.Remove(_propertyListList.Last());

            CurateProperties();
            return CreateQueryFromProperties();
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

        private string CreateQueryFromProperties()
        {
            var variableInputTypeName = (Attribute.GetCustomAttribute(_requestObjectType, typeof(VariableInputTypeAttribute)) as VariableInputTypeAttribute)?.ModelInputName;
            if (string.IsNullOrEmpty(variableInputTypeName))
            {
                var errorMessage = $"No {nameof(VariableInputTypeAttribute)} found. Please ensure the model has been marked or the value is not null";
                throw new RequestException(errorMessage);
            }

            var methodString = $"($variable: {variableInputTypeName}!)" + $"{{{MutationName}({MutationParameterName}: $variable)";
            var variableString = GetVariableString();
            var finalQuery = $"mutation{methodString}{variableString}";
            return finalQuery;
        }

        private string GetVariableString()
        {
            string queryString = "";

            foreach (List<PropertyDetail> propertyList in _propertyListList.Where(list => list.Any()))
            {
                propertyList.Reverse();
                foreach (PropertyDetail property in propertyList)
                {
                    queryString = queryString.Any() ? queryString.Insert(0, ToLowerFirstChar(property?.FieldName) + " ") : queryString.Insert(0, ToLowerFirstChar(property?.FieldName));
                }
                queryString = "{" + $"{queryString}" + "}";
            }
            return queryString + "}";
        }
    }
}
