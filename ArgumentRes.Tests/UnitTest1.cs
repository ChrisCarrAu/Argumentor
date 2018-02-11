using ArgumentRes.Models;
using System.Collections.Generic;
using Xunit;

namespace ArgumentRes.Tests
{
    public class CommandLineParserTest
    {
        [Fact]
        public void OnParse_ParsesCorrectly()
        {
            var argumentor = new Argumentor();
            argumentor.AddSwitch("c", "The number of pings to send", Required.Mandatory);
            argumentor.AddSwitch("t", "Tengu maru", Required.Optional);
            argumentor.AddSwitch("u", "Umbrella count for those days when the rain doth pour in buckets and the hoi polloi do cower in corners and alleyways in fear of becoming wettened and soggy", Required.Optional);
            argumentor.AddSwitch("v", "Victor is an exceedingly handsome fellow", Required.Mandatory);
            argumentor.AddSwitch("w", "With enough help, anything is possible", Required.Optional);
            argumentor.AddArgument("host", "The name of the host", Required.Mandatory);
            argumentor.AddArguments("files to process", Required.Mandatory);

            var arguments = argumentor.Parse(Args());

            Assert.Equal("100", arguments["c"]);
            Assert.Equal("tinkle", arguments["v"]);
            Assert.Equal("finagle", arguments["host"]);
            Assert.Equal("Oh", arguments["1"]);
            Assert.Equal("freddled", arguments["2"]);
            Assert.Equal("gruntbuggly,:", arguments["3"]);
            Assert.Equal("bee,", arguments["17"]);
        }

        private IEnumerable<string> Args()
        {
            return new List<string>
            {
                "-c",
                "100",
                "finagle",
                "-v",
                "tinkle",
                "Oh",
                "freddled",
                "gruntbuggly,:",
                "Thy",
                "micturations",
                "are",
                "to",
                "me,(with",
                "big",
                "yawning):",
                "As",
                "plurdled",
                "gabbleblotchits,:",
                "On",
                "a",
                "lurgid",
                "bee,",
            };
        }
    }
}
