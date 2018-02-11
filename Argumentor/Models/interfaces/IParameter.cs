namespace ArgumentRes.Models.interfaces
{
    public enum Required
    {
        Optional,
        Mandatory
    };

    public interface IParameter
    {
        /// <summary>
        /// Description of the parameter - used in Usage display to explain what the parameter does
        /// </summary>
        string FriendlyDescription { get; set; }

        /// <summary>
        /// Is this parameter mandatory or optional?
        /// </summary>
        Required IsRequired { get; set; }

        /// <summary>
        /// Returns the identifier of this parameter, used in error output to identify this parameter as invalid or missing
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Returns the name of this parameter in the display where an error condition occurs.
        /// </summary>
        string AsParam { get; }

        /// <summary>
        /// Returns the name of this parameter and the corresponding description for the Usage output
        /// </summary>
        string UsageString(int paramLength);
    }
}