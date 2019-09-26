using System;
namespace Xablu.WebApiClient.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class NameOfItemAttribute : Attribute
    {
        public NameOfItemAttribute(string[] values = null) => Values = values;

        public string[] Values { get; }
    }
}
