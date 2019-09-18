using System;
using System.Diagnostics.Contracts;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Xablu.WebApiClient.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class QueryParameter : Attribute
    {
        public QueryParameter()
        {

        }
        public QueryParameter(string exclusiveWith = null)
        {

        }
    }
}
