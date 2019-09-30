using System;
namespace Xablu.WebApiClient.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class QueryParameterAttribute : Attribute
    {
        public QueryParameterAttribute()
        {
        }
        public QueryParameterAttribute(string exclusiveWith = null) => ExclusiveWith = exclusiveWith;

        public string ExclusiveWith { get; }
    }
}
