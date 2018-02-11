namespace ArgumentRes.Models.implementations
{
    /// <summary>
    /// A switch is a command line parameter identified with a switch tag.
    /// </summary>
    /// <inheritdoc cref="Parameter"/>
    public class Switch : Parameter
    {
        protected string SwitchFlag = "-";

        public string Param { get; set; }
        public bool HasValue { get; set; } = true;

        public override string Id => Param;
        public override string AsParam => $"{OpenMandatoryFlag}{SwitchFlag}{Param} value{CloseMandatoryFlag}";
        public override string UsageString(int paramLength) => $"{Param.PadRight(paramLength)} : {FriendlyDescription}";
    }
}
