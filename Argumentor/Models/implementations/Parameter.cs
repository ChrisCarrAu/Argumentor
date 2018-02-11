using ArgumentRes.Models.interfaces;

namespace ArgumentRes.Models.implementations
{
    /// <summary>
    /// Represents a command line parameter
    /// </summary>
    public abstract class Parameter : ParameterImpl, IParameter
    {
        /// <summary>
        /// Returns the opening symbol to display a mandatory flag for Mandatory parameters, else a blank string - used in the Usage output
        /// </summary>
        protected string OpenMandatoryFlag => IsRequired == Required.Mandatory ? "" : "[";
        /// <summary>
        /// Returns the closing symbol to display a mandatory flag for Mandatory parameters, else a blank string - used in the Usage output
        /// </summary>
        protected string CloseMandatoryFlag => IsRequired == Required.Mandatory ? "" : "]";

    }
}
