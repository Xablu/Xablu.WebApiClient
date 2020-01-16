using System;
namespace Xablu.WebApiClient.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class VariableInputAttribute : Attribute
    {
        public VariableInputAttribute(string VariableInputModelName = "")
        {
            if (!string.IsNullOrEmpty(VariableInputModelName))
            {
                ModelInputName = VariableInputModelName;
            }
        }

        public string ModelInputName { get; }
    }
}
