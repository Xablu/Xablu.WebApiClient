using System;
namespace Xablu.WebApiClient.Attributes
{
    //WIP
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class QueryParameterAttribute : Attribute
    {
        public QueryParameterAttribute()
        {
        }
    }
}
