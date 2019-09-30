using System;
namespace Xablu.WebApiClient.Attributes
{ 
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = true)]
    public class GraphQLEndpointAttribute : Attribute
    {
        public GraphQLEndpointAttribute(string path)
        {
            Path = path;
        }

        public string Path { get; private set; }
    }
}
