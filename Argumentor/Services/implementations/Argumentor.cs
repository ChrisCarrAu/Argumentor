using ArgumentRes.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArgumentRes.Services.implementations
{
    /// <summary>
    /// Converts command line arguments into an object of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Argumentor<T> : IArgumentor<T> where T : new()
    {
        /// <summary>
        /// Switches and Arguments that are expected to be present.
        /// </summary>
        private readonly string _switchTag;

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
        public Argumentor(string switchTag)
        {
            _switchTag = switchTag;
        }

        /// <inheritdoc />
        public T Parse(IEnumerable<string> args)
        {
            // Value to return after successful parsing.
            var returnValue = new T();
            var properties = new Properties<T>();

            string flag = null;
            foreach (var arg in args)
            {
                if (null != flag)
                {
                    // this flag is set to arg
                    properties.SetFlagValue(returnValue, flag, arg);
                    flag = null;
                }
                else if (arg.StartsWith(_switchTag, System.StringComparison.Ordinal))
                {
                    // This is a switch - strip the switch flag from front
                    flag = arg.Substring(_switchTag.Length);
                    if (properties.IsBooleanSwitch(flag))
                    {
                        properties.SetFlagValue(returnValue, flag, true);
                        flag = null;
                    }
                }
                else
                {
                    properties.SetPropertyValue(returnValue, arg);
                }
            }

            // If all mandatory arguments have been set, then this list should be empty
            if (properties.MissingArguments.Any())
            {
                throw new ArgumentException("Expecting parameter/s " + properties.MissingArguments.Aggregate((current, next) => $"{current}, {next}"));
            }

            return returnValue;
        }
    }
}
