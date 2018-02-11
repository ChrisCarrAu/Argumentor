using System.Dynamic;

namespace ArgumentRes.Models
{
    /// <summary>
    /// Represents a command line parameter
    /// </summary>
    public abstract class Parameter : IParameter
    {
        /// <summary>
        /// Description of the parameter - used in Usage display to explain what the parameter does
        /// </summary>
        public string FriendlyDescription { get; set; }

        /// <summary>
        /// Is this parameter mandatory or optional?
        /// </summary>
        public Required IsRequired { get; set; }

        /// <summary>
        /// Returns the opening symbol to display a mandatory flag for Mandatory parameters, else a blank string - used in the Usage output
        /// </summary>
        protected string OpenMandatoryFlag => IsRequired == Required.Mandatory ? "" : "[";
        /// <summary>
        /// Returns the closing symbol to display a mandatory flag for Mandatory parameters, else a blank string - used in the Usage output
        /// </summary>
        protected string CloseMandatoryFlag => IsRequired == Required.Mandatory ? "" : "]";

        /// <summary>
        /// Returns the identifier of this parameter, used in error output to identify this parameter as invalid or missing
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Returns the name of this parameter in the display where an error condition occurs.
        /// </summary>
        public abstract string AsParam { get; }

        /// <summary>
        /// Returns the name of this parameter and the corresponding description for the Usage output
        /// </summary>
        public abstract string UsageString(int paramLength);
    }
}
