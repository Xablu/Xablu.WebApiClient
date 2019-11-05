using System;
namespace Xablu.WebApiClient.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NameOfFieldAttribute : Attribute
    {
        public NameOfFieldAttribute(string nameOfField = null) => NameOfField = nameOfField;

        public string NameOfField { get; }
    }
}
