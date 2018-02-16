using ArgumentRes.Attributes;
using ArgumentRes.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ArgumentRes.Services.implementations
{
    public class Properties<T> : IProperties<T>
    {
        private readonly IEnumerable<PropertyInfo> _properties;
        private readonly IDictionary<PropertyInfo, string> _mandatoryArguments;
        private int _propertyNumber;

        public Properties()
        {
            // Get the properties on this class
            _properties = typeof(T).GetProperties();

            // Initialise mandatory arguments
            _mandatoryArguments = MandatoryArguments;
        }

        public PropertyInfo Switch(string key)
        {
            if (Flags.ContainsKey(key))
            {
                return Flags[key];
            }
            throw new ArgumentException($"Unknown parameter {key}");
        }

        /// <summary>
        /// Is the value passed a simple boolean switch with no argument? (ie. is the property for this key a bool?)
        /// </summary>
        /// <param name="param">Property key</param>
        /// <returns>True if the property is boolean</returns>
        public bool IsBooleanSwitch(string param)
        {
            return Flags.ContainsKey(param) && Flags[param].PropertyType == typeof(bool);
        }

        public IEnumerable<string> MissingArguments => _mandatoryArguments.Select(arg => arg.Value);

        public void SetFlagValue(T obj, string param, object arg)
        {
            // Expecting a switch parameter
            var propertyInfo = Switch(param);
            var type = propertyInfo.PropertyType;

            try
            {
                // Attempt to convert the string value into the property type
                var value = Convert.ChangeType(arg, type);

                propertyInfo.SetValue(obj, value, null);

                // Check off this mandatory argument
                _mandatoryArguments.Remove(propertyInfo);
            }
            catch (Exception e)
            {
                throw new Exception($"Unable to cast value {arg} for switch {param} to {type.Name}", e);
            }
        }

        public void SetPropertyValue(T obj, object arg)
        {
            // This is a simple text argument
            var propertyInfo = Parameters[_propertyNumber];
            if (propertyInfo.PropertyType == typeof(List<string>))
            {
                // The argument is a string list - create the list if required then add our argument to it
                var list = (List<string>)propertyInfo.GetValue(obj);
                if (null == list)
                {
                    list = new List<string>();
                    propertyInfo.SetValue(obj, list, null);
                }
                list.Add(arg as string);
            }
            else
            {
                // The argument is a simgple string value - just set the value
                propertyInfo.SetValue(obj, arg);
                _propertyNumber++;
            }

            // Check off this mandatory argument
            _mandatoryArguments.Remove(propertyInfo);
        }

        private IDictionary<string, PropertyInfo> Flags =>_properties
                .Where(p => p.GetCustomAttributes<FlagAttribute>().Any())
                .ToDictionary(t => t.GetCustomAttributes<FlagAttribute>().First().Key, t => t);

        private IList<PropertyInfo> Parameters => _properties
                .Where(p => p.GetCustomAttributes<ParameterAttribute>().Any())
                .ToList();

        private IDictionary<PropertyInfo, string> MandatoryArguments => _properties
                .Where(p => p.GetCustomAttributes<MandatoryAttribute>().Any())
                .ToDictionary(t => t, t => t.GetCustomAttributes<ParameterAttribute>().FirstOrDefault()?.Key);
    }
}
