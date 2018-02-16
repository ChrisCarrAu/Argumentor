using ArgumentRes.Attributes;
using ArgumentRes.Services.implementations;
using Xunit;

namespace ArgumentRes.Tests.UnitTests
{
    public class BooleanArgumentTest
    {
        internal class Arguments_BoolSwitch
        {
            [Flag(Key = "switch", Description = "Switch details")]
            public bool Switch { get; set; }
        }

        [Fact]
        public void OnParse_BooleanSwitchPassed_ParsesTrue()
        {
            var argumentor = new Argumentor<Arguments_BoolSwitch>();
            var arguments = argumentor.Parse(new string[] { "-switch" });

            Assert.True(arguments.Switch);
        }

        [Fact]
        public void OnParse_BooleanSwitchNotPassed_ParsesFalse()
        {
            var argumentor = new Argumentor<Arguments_BoolSwitch>();
            var arguments = argumentor.Parse(new string[] { });

            Assert.False(arguments.Switch);
        }
    }
}
