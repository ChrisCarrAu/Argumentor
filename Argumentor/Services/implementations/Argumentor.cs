using ArgumentRes.Attributes;
using ArgumentRes.Models;
using ArgumentRes.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ArgumentRes.Services.implementations
{
    public class Argumentor<T> : IArgumentor<T> where T : new()
    {
        private const int Defaultscreenwidth = 80;

        /// <summary>
        /// Switches and Arguments that are expected to be present.
        /// </summary>
        private readonly string _switchTag;

        /// <summary>
        /// Width of screen
        /// </summary>
        public int ScreenWidth { get; private set; }

        /// <summary>
        /// Default constructor - sets switch flag to a hyphen (-)
        /// </summary>
        public Argumentor() : this("-")
        {
        }

        /// <summary>
        /// Constructor with ability to set the switch flag
        /// </summary>
        /// <param name="switchTag">Tag to use for switches - defaults to hyphen (-)</param>
        /// <param name="screenWidth">Screen width (to wrap usage comments) defaults to 80</param>
        public Argumentor(string switchTag, int screenWidth = Defaultscreenwidth)
        {
            _switchTag = switchTag;
            ScreenWidth = screenWidth;
        }

        /// <inheritdoc />
        public T Parse(IEnumerable<string> args)
        {
            // Value to return after successful parsing.
            var returnValue = new T();
            var properties = returnValue.GetType().GetProperties();

            var commandLineSwitches = properties.Where(p => p.GetCustomAttributes<SwitchAttribute>().Any())
                .ToDictionary(t => t.GetCustomAttributes<SwitchAttribute>().First().Key, t => t);
            var commandLineParameters = properties.Where(p => p.GetCustomAttributes<ParameterAttribute>().Any())
                .ToList();
            var mandatoryArguments = properties.Where(p => p.GetCustomAttributes<MandatoryAttribute>().Any())
                .ToDictionary(t => t, t => t.GetCustomAttributes<ParameterAttribute>().FirstOrDefault()?.Key);

            var propertyNumber = 0;
            string param = null;
            foreach (var arg in args)
            {
                if (null != param)
                {
                    // Expecting a switch parameter
                    var propertyInfo = commandLineSwitches[param];

                    object value;
                    var type = propertyInfo.PropertyType;

                    try
                    {
                        // Attempt to convert the string value into the property type
                        value = Convert.ChangeType(arg, type);
                    }
                    catch (Exception e)
                    {
                        throw new Exception($"Unable to cast value {arg} for switch {_switchTag}{param} to {type.Name}", e);
                    }

                    propertyInfo.SetValue(returnValue, value, null);
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
                throw new ArgumentException("Expecting parameter/s " + mandatoryArguments.Select(arg => arg.Value).Aggregate((current, next) => $"{current}, {next}"));
            }

            return returnValue;
        }

        /// <summary>
        /// Uses the application process name as the command name.
        /// </summary>
        /// <returns></returns>
        public string Usage()
        {
            return Usage(System.AppDomain.CurrentDomain.FriendlyName);
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
                arg.Description = Wrap(arg.Description, maxParamLength + 3, ScreenWidth);
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
