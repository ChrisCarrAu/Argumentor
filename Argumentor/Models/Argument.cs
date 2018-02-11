namespace ArgumentRes.Models
{
    /// <summary>
    /// An argument is a solitary command line parameter which is not a switch
    /// </summary>
    public class Argument : Parameter
    {
        /// <summary>
        /// The name of the parameter 
        /// </summary>
        public string ShortName { get; set; }

        /// <summary>
        /// Returns the identifier of this parameter, used in error output to identify this parameter as invalid or missing
        /// </summary>
        public override string Id => ShortName;
        /// <summary>
        /// Returns the name of this parameter in the display where an error condition occurs.
        /// </summary>
        public override string AsParam => $"{ShortName}";
        /// <summary>
        /// Returns the name of this parameter and the corresponding description for the Usage output
        /// </summary>
        public override string UsageString(int paramLength) => $"{ShortName.PadRight(paramLength)} : {FriendlyDescription}";
    }
}
