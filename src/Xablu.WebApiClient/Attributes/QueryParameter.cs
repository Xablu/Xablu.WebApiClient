using System;
namespace Xablu.WebApiClient.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class QueryParameter : Attribute
    {
        public QueryParameter()
        {

        }
        public QueryParameter(string exclusiveWith = null) => ExclusiveWith = exclusiveWith;

        public string ExclusiveWith { get; }
    }
}
