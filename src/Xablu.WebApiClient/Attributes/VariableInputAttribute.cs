using System;
namespace Xablu.WebApiClient.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class VariableInputAttribute : Attribute
    {
        public VariableInputAttribute(string VariableInputModelName = "")
        {
            if (string.IsNullOrEmpty(VariableInputModelName))
            {
                ModelInputName = VariableInputModelName;
            }
            else
            {
                ModelInputName = GetType().Name;
            }


        }

        public string ModelInputName { get; }
    }
}
