using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ArgumentRes.Models;
using ArgumentRes.Models.implementations;
using ArgumentRes.Models.interfaces;
using ArgumentRes.Services.interfaces;
using Switch = ArgumentRes.Models.implementations.Switch;

namespace ArgumentRes.Services.implementations
{
    public class Argumentor : IArgumentor
    {
        /// <summary>
        /// Switches and Arguments that are expected to be present.
        /// </summary>
        private readonly List<IParameter> _expectedArguments = new List<IParameter>();
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

        /// <summary>
        /// Add an expected switch parameter
        /// </summary>
        /// <param name="param">Parameter id - this is used to identify the parameter</param>
        /// <param name="shortDescription"></param>
        /// <param name="friendlyDescription"></param>
        /// <param name="required"></param>
        public Switch AddSwitch(string param, string friendlyDescription, Required required)
        {
            var switchX = new Switch {Param = param, FriendlyDescription = friendlyDescription, IsRequired = required};
            _expectedArguments.Add(switchX);
            return switchX;
        }

        /// <summary>
        /// Adds an expected command line argument parameter
        /// </summary>
        /// <param name="shortDescription"></param>
        /// <param name="friendlyDescription"></param>
        /// <param name="required"></param>
        public void AddArgument(string shortDescription, string friendlyDescription, Required required)
        {
            if (ExpectsArgumentList)
            {
                throw new ArgumentException("Argument List must be the last argument exoected");
            }

            _expectedArguments.Add(new Argument { ShortName = shortDescription, FriendlyDescription = friendlyDescription, IsRequired = required });
        }

        /// <summary>
        /// Adds expected command line arguments
        /// </summary>
        /// <param name="friendlyDescription">Description of the command line arguments</param>
        /// <param name="required">if optional, expects 0 or more parameters. if mandatory, expects 1 or more parameters</param>
        public void AddArguments(string friendlyDescription, Required required)
        {
            if (ExpectsArgumentList)
            {
                throw new ArgumentException("Only one argument list is permitted.");
            }

            _expectedArguments.Add(new ArgumentList { FriendlyDescription = friendlyDescription, IsRequired = required });
        }

        /// <summary>
        /// Returns true if 
        /// </summary>
        private bool ExpectsArgumentList => _expectedArguments.OfType<ArgumentList>().Any();

        /// <inheritdoc />
        public ParsedArguments Parse(IEnumerable<string> args)
        {
            var commandLineSwitches = new Dictionary<string, string>();
            var commandLineParameters = new Dictionary<string, string>();

            string param = null;
            foreach (var arg in args)
            {
                if (null != param)
                {
                    commandLineSwitches[param] = arg;
                    param = null;
                }
                else if (arg.StartsWith(_switchTag, System.StringComparison.Ordinal))
                {
                    // Switch
                    param = arg.Substring(1);
                    if (_expectedArguments.OfType<Switch>().Any(a => a.Param.Equals(param) && !a.HasValue))
                    {
                        // Switch has no value
                        commandLineSwitches[param] = "";
                        param = null;
                    }
                }
                else
                {
                    var expectedArgs = _expectedArguments.OfType<Argument>().Select(argv => argv.ShortName).ToList();
                    var nextParam = commandLineParameters.Count;
                    if (nextParam < expectedArgs.Count)
                    {
                        commandLineParameters[expectedArgs[nextParam]] = arg;
                    }
                    else if (ExpectsArgumentList)
                    {
                        commandLineParameters[nextParam.ToString()] = arg;
                    }
                }
            }

            var missingSwitches = _expectedArguments
                .OfType<Switch>()
                .Where(arg => arg.IsRequired == Required.Mandatory && !commandLineSwitches.ContainsKey(arg.Param))
                .Select(arg => arg.Id).ToList();

            if (missingSwitches.Any())
            {
                throw new ArgumentException("Expecting switches: " + missingSwitches.Aggregate((current, next) => $"{current}, {next}"));
            }
            var expectedArguments = _expectedArguments.OfType<Argument>().Select(arg => arg.ShortName).ToList();
            var expectedArgumentList = _expectedArguments.OfType<ArgumentList>().FirstOrDefault();

            if (null != expectedArgumentList)
            {
                var argListMinArgs = expectedArgumentList.IsRequired == Required.Mandatory ? 1 : 0;
                if (expectedArguments.Count + argListMinArgs > commandLineParameters.Count)
                {
                    var missingParameters = expectedArguments.Skip(commandLineParameters.Count).ToList();
                    if (missingParameters.Count() > 1)
                    {
                        throw new ArgumentException("Expecting parameter/s: " +
                                                    missingParameters.Aggregate((current, next) =>
                                                        $"{current}, {next}"));
                    }
                    else
                    {
                        throw new ArgumentException("Expecting parameter");

                    }
                }
            }
            else if (expectedArguments.Count < commandLineParameters.Count)
            {
                throw new ArgumentException("Invalid parameter/s: " + commandLineParameters.Skip(expectedArguments.Count).Select(arg => arg.Key).Aggregate((current, next) => $"{current}, {next}"));
            }
            else if (expectedArguments.Count > commandLineParameters.Count)
            {
                throw new ArgumentException("Expecting parameter/s: " + expectedArguments.Skip(commandLineParameters.Count).Aggregate((current, next) => $"{current}, {next}"));
            }

            return new ParsedArguments(commandLineSwitches, commandLineParameters);
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
            var maxParamLength = _expectedArguments.Max(arg => arg.Id.Length);

            return $"{command} " 
                + _expectedArguments.Select(arg => arg.AsParam).Aggregate((current, next) => $"{current} {next}") 
                + "\n "
                + _expectedArguments.Select(arg => arg.UsageString(maxParamLength)).Aggregate((current, next) => $"{current}\n {next}");
        }
    }
}
