using System;

namespace Yokogawa.IIoT.AzureHttpFunctionSwagger
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class SwaggerRequestAttribute : Attribute
    {
        public SwaggerRequestAttribute(bool required = true)
        {
            Required = required;
        }

        public string Name { get; set; }
        public RequestSource In { get; set; }
        public bool Required { get; set; }
        public Type Type { get; set; }
        public string Description { get; set; }
    }
}