using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ArgumentRes.Models
{
    public class ParsedArguments
    {
        /// <summary>
        /// Switches found
        /// </summary>
        private readonly ReadOnlyDictionary<string, string> _commandLineSwitches;
        /// <summary>
        /// Arguments found
        /// </summary>
        private readonly ReadOnlyDictionary<string, string> _commandLineParameters;

        /// <summary>
        /// Constructor, creates a new ParsedArguments object with the results of a Parse operation
        /// </summary>
        /// <param name="commandLineSwitches"></param>
        /// <param name="commandLineParameters"></param>
        public ParsedArguments(IDictionary<string, string> commandLineSwitches, IDictionary<string, string> commandLineParameters)
        {
            _commandLineSwitches = new ReadOnlyDictionary<string, string>(commandLineSwitches);
            _commandLineParameters = new ReadOnlyDictionary<string, string>(commandLineParameters);
        }

        /// <summary>
        /// Returns the value associated with the switch id or the parameters ordinal position
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public string this[string param] => _commandLineSwitches.ContainsKey(param) ? _commandLineSwitches[param] : _commandLineParameters[param];

        public IEnumerable<KeyValuePair<string, string>> GetSwitches()
        {
            return _commandLineSwitches;
        }

        public IEnumerable<KeyValuePair<string, string>> GetParameters()
        {
            return _commandLineParameters;
        }

    }
}
