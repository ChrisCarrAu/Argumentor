using System.Collections.Generic;
using ArgumentRes.Models.interfaces;

namespace ArgumentRes.Models.implementations
{
    public class ArgumentList : ParameterImpl, IParameter
    {
        private List<Argument> _arguments;

        /// <inheritdoc />
        public override string Id => "...";
        /// <inheritdoc />
        public override string AsParam => "...";
        /// <inheritdoc />
        public override string UsageString(int paramLength) => $"{"...".PadRight(paramLength)} : {FriendlyDescription}";

    }
}
