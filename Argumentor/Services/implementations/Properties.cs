using ArgumentRes.Attributes;
using ArgumentRes.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ArgumentRes.Services.implementations
{
    /// <summary>
    /// Manages the setting of properties of an object of type T based on the argument attributes 
    /// on the class.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Properties<T> : IProperties<T>
    {
        private readonly IEnumerable<PropertyInfo> _properties;
        private readonly IDictionary<PropertyInfo, string> _mandatoryArguments;
        private int _propertyNumber;

        /// <summary>
        /// Default constructor
        /// </summary>
        public Properties()
        {
            // Get the properties on this class
            _properties = typeof(T).GetProperties();

            // Initialise mandatory arguments
            _mandatoryArguments = MandatoryArguments;
        }

        /// <summary>
        /// Returns the property info object for a switch with the given key. If key is not valid, 
        /// throws an argument exception.
        /// </summary>
        /// <param name="key">Switch key to set</param>
        /// <returns>Related property info object</returns>
        public PropertyInfo Switch(string key)
        {
            if (Flags.ContainsKey(key))
            {
                return Flags[key];
            }
            throw new ArgumentException($"Unknown parameter {key}");
        }

        /// <summary>
        /// Is the value passed a simple boolean switch with no argument? (ie. is the property
        ///  for this key a bool?)
        /// </summary>
        /// <param name="param">Property key</param>
        /// <returns>True if the property is boolean</returns>
        public bool IsBooleanSwitch(string param)
        {
            return Flags.ContainsKey(param) && Flags[param].PropertyType == typeof(bool);
        }

        /// <summary>
        /// Returns a list of missing arguments. As arguments are provided to SetFlagValue, 
        /// any that are mandatory are removed from this collection
        /// </summary>
        public IEnumerable<string> MissingArguments => _mandatoryArguments.Select(arg => arg.Value);

        /// <summary>
        /// Sets the value of a property
        /// </summary>
        /// <param name="obj">Instance of a T with properties matching the arguments</param>
        /// <param name="param">Flag name of the Flag property</param>
        /// <param name="arg">Value to set on the Flag property</param>
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

        /// <summary>
        /// Sets the value of the next parameter type property of an object of type T.
        /// </summary>
        /// <param name="obj">Instance of a T with parameter properties</param>
        /// <param name="arg">Value of this parameter</param>
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

        /// <summary>
        /// Helper function to get the Flag properties
        /// </summary>
        private IDictionary<string, PropertyInfo> Flags =>_properties
                .Where(p => p.GetCustomAttributes<FlagAttribute>().Any())
                .ToDictionary(t => t.GetCustomAttributes<FlagAttribute>().First().Key, t => t);
        /// <summary>
        /// Helper function to get a list of parameter properties
        /// </summary>
        private IList<PropertyInfo> Parameters => _properties
                .Where(p => p.GetCustomAttributes<ParameterAttribute>().Any())
                .ToList();
        /// <summary>
        /// Helper function that returns properties identified as mandatory
        /// </summary>
        private IDictionary<PropertyInfo, string> MandatoryArguments => _properties
                .Where(p => p.GetCustomAttributes<MandatoryAttribute>().Any())
                .ToDictionary(t => t, t => t.GetCustomAttributes<ParameterAttribute>().FirstOrDefault()?.Key);
    }
}
