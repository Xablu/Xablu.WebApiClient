using System;
namespace Xablu.WebApiClient.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class QueryParameterAttribute : Attribute
    {
        public QueryParameterAttribute()
        {
        }
    }
}
