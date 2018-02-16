using ArgumentRes.Attributes;
using ArgumentRes.Models;
using ArgumentRes.Services.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ArgumentRes.Services.implementations
{
    public class UsageFormatter<T> : IUsageFormatter<T>
    {
        private const int Defaultscreenwidth = 80;
        private const string Defaultswitchflag = "-";

        /// <summary>
        /// Switches and Arguments that are expected to be present.
        /// </summary>
        private readonly string _switchTag;

        /// <summary>
        /// Width of screen
        /// </summary>
        public int ScreenWidth { get; private set; }

        /// <summary>
        /// Constructor with ability to set the switch flag
        /// </summary>
        /// <param name="switchTag">Tag to use for switches - defaults to hyphen (-)</param>
        /// <param name="screenWidth">Screen width (to wrap usage comments) defaults to 80</param>
        public UsageFormatter(string switchTag = Defaultswitchflag, int screenWidth = Defaultscreenwidth)
        {
            _switchTag = switchTag;
            ScreenWidth = screenWidth;
        }

        /// <summary>
        /// Uses the application process name as the command name.
        /// </summary>
        /// <returns></returns>
        public new string ToString()
        {
            return ToString(AppDomain.CurrentDomain.FriendlyName);
        }

        /// <inheritdoc />
        public string ToString(string command)
        {
            var arguments = new List<Argument>();

            // Iterate the properties of T to get the arguments and other flags
            var properties = typeof(T).GetProperties();
            foreach (var property in properties.Where(p => p.GetCustomAttributes(true).Any()))
            {
                var attributes = property.GetCustomAttributes(true);

                var argumentAttribute = attributes.OfType<ArgumentAttribute>().FirstOrDefault();
                var mandatoryAttribute = attributes.OfType<MandatoryAttribute>().FirstOrDefault();

                var mandatory = (null != mandatoryAttribute);
                var name = property.Name;

                if (property.PropertyType == typeof(bool)) name = "";

                // Add this parameter with it's attribute info to the list of arguments
                arguments.Add(new Argument
                {
                    IsSwitch = (argumentAttribute is FlagAttribute),
                    Key = (argumentAttribute is FlagAttribute ? _switchTag : "") + argumentAttribute.Key,
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
        /// Wrap the description text at a word break and include indentation for subsequent lines
        /// </summary>
        /// <param name="description"></param>
        /// <param name="indent"></param>
        /// <param name="maxWidth"></param>
        /// <returns></returns>
        private string Wrap(string description, int indent, int maxWidth)
        {
            var charCount = 0;
            var lines = description.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .GroupBy(w => (charCount += w.Length + 1) / (maxWidth - indent))
                .Select(g => string.Join(" ", g));

            return lines.Aggregate((current, next) => $"{current}{"\n".PadRight(indent + 1)}{next}");
        }
    }
}
