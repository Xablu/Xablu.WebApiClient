using System;

namespace Xablu.WebApiClient.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class VariableInputTypeAttribute : Attribute
    {
        public VariableInputTypeAttribute(string variableInputModelName = "")
        {
            if (!string.IsNullOrEmpty(variableInputModelName))
            {
                ModelInputName = variableInputModelName;
            }
        }

        public string ModelInputName { get; }
    }
}
