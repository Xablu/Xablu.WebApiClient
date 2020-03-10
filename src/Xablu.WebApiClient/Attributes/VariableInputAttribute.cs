using System;
namespace Xablu.WebApiClient.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class VariableInputTypeAttribute : Attribute
    {
        public VariableInputTypeAttribute(string VariableInputModelName = "")
        {
            if (!string.IsNullOrEmpty(VariableInputModelName))
            {
                ModelInputName = VariableInputModelName;
            }
        }

        public string ModelInputName { get; }
    }
}
