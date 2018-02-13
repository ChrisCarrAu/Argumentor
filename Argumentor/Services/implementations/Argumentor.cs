using ArgumentRes.Attributes;
using ArgumentRes.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ArgumentRes.Services.interfaces;
using SwitchAttribute = ArgumentRes.Attributes.SwitchAttribute;

namespace ArgumentRes.Services.implementations
{
    public class Argumentor<T> : IArgumentor<T> where T : new()
    {
        /// <summary>
        /// Switches and Arguments that are expected to be present.
        /// </summary>
        //private readonly List<IParameter> _expectedArguments = new List<IParameter>();
        private readonly string _switchTag;

        /// <summary>
        /// Default constructor - sets switch flag to -
        /// </summary>
        public Argumentor() : this("-")
        {
        }

        /// <summary>
        /// Constructor with ability to set the switch flag
        /// </summary>
        /// <param name="switchTag"></param>
        public Argumentor(string switchTag)
        {
            _switchTag = switchTag;
        }

        /// <inheritdoc />
        public T Parse(IEnumerable<string> args)
        {
            var commandLineSwitches = new Dictionary<string, PropertyInfo>();
            var commandLineParameters = new List<PropertyInfo>();
            var mandatoryArguments = new List<PropertyInfo>();

            // Value to return after successful parsing.
            var returnValue = new T();
            var properties = returnValue.GetType().GetProperties();

            // Build up the attributes for each property on T
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(true);

                var switchAttribute = attributes.OfType<SwitchAttribute>().FirstOrDefault();
                var parameterAttribute = attributes.OfType<ParameterAttribute>().FirstOrDefault();
                var mandatoryAttribute = attributes.OfType<MandatoryAttribute>().FirstOrDefault();

                if (null != switchAttribute)
                { 
                    commandLineSwitches.Add(switchAttribute.Key, property);
                }
                if (null != parameterAttribute)
                {
                    commandLineParameters.Add(property);
                }
                if (null != mandatoryAttribute)
                {
                    mandatoryArguments.Add(property);
                }
            }

            var propertyNumber = 0;
            string param = null;
            foreach (var arg in args)
            {
                if (null != param)
                {
                    // Expecting a switch parameter
                    var propertyInfo = commandLineSwitches[param];
                    propertyInfo.SetValue(returnValue, Convert.ChangeType(arg, propertyInfo.PropertyType), null);
                    param = null;

                    // Check off this mandatory argument
                    mandatoryArguments.Remove(propertyInfo);
                }
                else if (arg.StartsWith(_switchTag, System.StringComparison.Ordinal))
                {
                    // This is a switch - strip the switch flag from front
                    param = arg.Substring(1);
                    if (commandLineSwitches.ContainsKey(param) && commandLineSwitches[param].PropertyType == typeof(bool))
                    {
                        // This is a boolean switch, there is no value - just set to true so we know it was set.
                        var propertyInfo = commandLineSwitches[param];
                        propertyInfo.SetValue(returnValue, true);
                        param = null;

                        // Check off this mandatory argument
                        mandatoryArguments.Remove(propertyInfo);
                    }
                }
                else
                {
                    // This is a simple text argument
                    var propertyInfo = commandLineParameters[propertyNumber];
                    if (propertyInfo.PropertyType == typeof(List<string>))
                    {
                        // The argument is a string list - create the list if required then add our argument to it
                        var list = (List<string>) propertyInfo.GetValue(returnValue);
                        if (null == list)
                        {
                            list = new List<string>();
                            propertyInfo.SetValue(returnValue, list, null);
                        }
                        list.Add(arg);
                    }
                    else
                    {
                        // The argument is a simgple string value - just set the value
                        propertyInfo.SetValue(returnValue, arg);
                        propertyNumber++;
                    }

                    // Check off this mandatory argument
                    mandatoryArguments.Remove(propertyInfo);
                }
            }

            // If all mandatory arguments have been set, then this list should be empty
            if (mandatoryArguments.Count > 0)
            {
                throw new ArgumentException("Expecting parameter/s " + mandatoryArguments.Select(arg => arg.Name).Aggregate((current, next) => $"{current}, {next}"));
            }

            return returnValue;
        }

        /// <summary>
        /// Uses the application process name as the command name.
        /// </summary>
        /// <returns></returns>
        public string Usage()
        {
            return Usage(Process.GetCurrentProcess().ProcessName);
        }

        /// <inheritdoc />
        public string Usage(string command)
        {
            var arguments = new List<Argument>();

            // Iterate the properties of T to get the arguments and other flags
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(true);

                var argumentAttribute = attributes.OfType<ArgumentAttribute>().FirstOrDefault();
                var mandatoryAttribute = attributes.OfType<MandatoryAttribute>().FirstOrDefault();

                if (null == argumentAttribute)
                    continue;

                var mandatory = (null != mandatoryAttribute);
                var name = property.Name;

                if (property.PropertyType == typeof(bool)) name = "";

                // Add this parameter with it's attribute info to the list of arguments
                arguments.Add(new Argument
                {
                    IsSwitch = (argumentAttribute is SwitchAttribute),
                    Key = (argumentAttribute is SwitchAttribute ? _switchTag : "") + argumentAttribute.Key,
                    Name = name,
                    Description = argumentAttribute.Description,
                    Mandatory = mandatory,
                });
            }

            // Left align the list
            var maxParamLength = arguments.Max(arg => arg.Key.Length);

            foreach (var arg in arguments)
            {
                arg.Description = Wrap(arg.Description, maxParamLength + 3, 80);
            }

            return $"{command} " 
                + arguments.Select(arg => arg.AsParam).Aggregate((current, next) => $"{current} {next}") 
                + "\n\n "
                + arguments.Select(arg => arg.UsageString(maxParamLength)).Aggregate((current, next) => $"{current}\n {next}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="description"></param>
        /// <param name="indent"></param>
        /// <param name="maxWidth"></param>
        /// <returns></returns>
        public string Wrap(string description, int indent, int maxWidth)
        {
            var charCount = 0;
            var lines = description.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .GroupBy(w => (charCount += w.Length + 1) / (maxWidth - indent))
                .Select(g => string.Join(" ", g));

            return lines.Aggregate((current, next) => $"{current}{"\n".PadRight(indent + 1)}{next}");
        }
    }
}
