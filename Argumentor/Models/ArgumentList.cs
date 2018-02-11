using System.Collections.Generic;

namespace ArgumentRes.Models
{
    public class ArgumentList : IParameter
    {
        private List<Argument> _arguments;

        /// <summary>
        /// Description of the parameter - used in Usage display to explain what the parameter does
        /// </summary>
        public string FriendlyDescription { get; set; }

        /// <summary>
        /// Is this parameter mandatory or optional?
        /// </summary>
        public Required IsRequired { get; set; }

        /// <summary>
        /// Returns the identifier of this parameter, used in error output to identify this parameter as invalid or missing
        /// </summary>
        public string Id => "...";
        /// <summary>
        /// Returns the name of this parameter in the display where an error condition occurs.
        /// </summary>
        public string AsParam => "...";
        /// <summary>
        /// Returns the name of this parameter and the corresponding description for the Usage output
        /// </summary>
        public string UsageString(int paramLength) => $"{"...".PadRight(paramLength)} : {FriendlyDescription}";

    }
}
