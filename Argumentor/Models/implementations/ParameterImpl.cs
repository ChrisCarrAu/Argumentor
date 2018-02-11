using ArgumentRes.Models.interfaces;

namespace ArgumentRes.Models.implementations
{
    public abstract class ParameterImpl : IParameter
    {
        /// <inheritdoc />
        public string FriendlyDescription { get; set; }

        /// <inheritdoc />
        public Required IsRequired { get; set; }

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
