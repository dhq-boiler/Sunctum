

using System;

namespace Sunctum.Domain.Models
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ConfigurationDataAttribute : Attribute
    {
        private object _defaultValue;

        public ConfigurationDataAttribute()
        { }

        public ConfigurationDataAttribute(object defaultValue)
        {
            _defaultValue = defaultValue;
        }
    }
}
