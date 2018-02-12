namespace ArgumentRes.Models
{
    /// <summary>
    /// An argument is a solitary command line parameter which is not a switch
    /// </summary>
    internal class Argument
    {
        /// <summary>
        /// Returns the identifier of this parameter, used in error output to identify this parameter as invalid or missing
        /// </summary>
        public bool IsSwitch { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Mandatory { get; set; }

        /// <summary>
        /// Returns the name of this parameter in the display where an error condition occurs.
        /// </summary>
        public string AsParam => IsSwitch ? Mandatory ? $"{Key} {Name}" : $"[{Key} {Name}]" : Mandatory ? $"{Key}" : $"[{Key}]";

        /// <summary>
        /// Returns the name of this parameter and the corresponding description for the Usage output
        /// </summary>
        public string UsageString(int paramLength) => $"{Key.PadRight(paramLength)} : {Description}";
    }
}
