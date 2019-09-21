using System;
namespace Xablu.WebApiClient.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class QueryName : Attribute
    {
        public QueryName(string[] values = null) => Values = values;

        public string[] Values { get; }
    }
}
