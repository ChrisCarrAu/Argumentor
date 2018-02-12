using ArgumentRes.Attributes;
using ArgumentRes.Services.implementations;
using System;
using System.Collections.Generic;
using Xunit;

namespace ArgumentRes.Tests.UnitTests
{
    public class CommandLineParserTest
    {
        public class Arguments_BoolSwitch
        {
            [Switch(Key = "switch", Description = "Switch details")]
            [Mandatory]
            public bool Switch { get; set; }
        }
        public class Arguments_OneOrMore
        {
            [Parameter(Key = "args", Description = "Arguments")]
            [Mandatory]
            public List<string> Arguments { get; set; }
        }
        public class Arguments_ZeroOrMore
        {
            [Parameter(Key = "args", Description = "Arguments")]
            public List<string> Arguments { get; set; }
        }
        public class Arguments_Complex
        {
            [Switch(Key = "c", Description = "The number of pings to send")]
            [Mandatory]
            public int Pings { get; set; }
            [Switch(Key = "t", Description = "Tengu maru")]
            public string TenguMaru { get; set; }
            [Switch(Key = "u", Description = "Umbrella count for those days when the rain doth pour in buckets and the hoi polloi do cower in corners and alleyways in fear of becoming wettened and soggy")]
            public string Umbrella { get; set; }
            [Mandatory]
            [Switch(Key = "v", Description = "Victor is an exceedingly handsome fellow")]
            public string Victor { get; set; }
            [Switch(Key = "w", Description = "With enough help, anything is possible")]
            public string Whisky { get; set; }
            [Parameter(Key = "host", Description = "The name of the host")]
            [Mandatory]
            public string Host { get; set; }
            [Parameter(Key = "args", Description = "Arguments")]
            public List<string> Arguments { get; set; }
        }


        [Fact]
        public void OnParse_OnlySwitchWithNoValue_ParsesCorrectly()
        {
            var argumentor = new Argumentor<Arguments_BoolSwitch>();
            var arguments = argumentor.Parse(new string[] {"-switch"});

            Assert.True(arguments.Switch);
        }

        [Fact]
        public void OnParse_OnlyArgumentsMandatoryWithOneArgument_ParsesCorrectly()
        {
            var argumentor = new Argumentor<Arguments_OneOrMore>();
            var arguments = argumentor.Parse(new string[] {"one-arg"});

            Assert.Equal("one-arg", arguments.Arguments[0]);
            Assert.Single(arguments.Arguments);
        }

        [Fact]
        public void OnParse_OnlyArgumentsMandatoryWithNoArgument_ThrowsArgumentException()
        {
            var argumentor = new Argumentor<Arguments_OneOrMore>();
            Exception ex = Assert.Throws<ArgumentException>(() => argumentor.Parse(new string[] { }));

            Assert.StartsWith("Expecting parameter", ex.Message);
        }

        [Fact]
        public void OnParse_OnlyArgumentsOptionalWithNoArgument_ParsesCorrectly()
        {
            var argumentor = new Argumentor<Arguments_ZeroOrMore>();
            var arguments = argumentor.Parse(new string[] {  });

            Assert.Null(arguments.Arguments);
        }

        [Fact]
        public void OnParse_OnlyArguments_ParsesCorrectly()
        {
            var argumentor = new Argumentor<Arguments_OneOrMore>();
            var arguments = argumentor.Parse(ArgumentsOnlyArgs());

            Assert.Equal("Oh", arguments.Arguments[0]);
            Assert.Equal("freddled", arguments.Arguments[1]);
            Assert.Equal("gruntbuggly,:", arguments.Arguments[2]);
            Assert.Equal("bee,", arguments.Arguments[16]);
            Assert.Equal(17, arguments.Arguments.Count);
        }
        
        [Fact]
        public void OnParse_ComplexArguments_ParsesCorrectly()
        {
            var argumentor = new Argumentor<Arguments_Complex>();
            var arguments = argumentor.Parse(ComplexArgs());

            Assert.Equal(100, arguments.Pings);
            Assert.Equal("tinkle", arguments.Victor);
            Assert.Equal("finagle", arguments.Host);
            Assert.Equal("Oh", arguments.Arguments[0]);
            Assert.Equal("freddled", arguments.Arguments[1]);
            Assert.Equal("gruntbuggly,:", arguments.Arguments[2]);
            Assert.Equal("bee,", arguments.Arguments[16]);

            Assert.Equal(17, arguments.Arguments.Count);
        }

        private IEnumerable<string> ComplexArgs()
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

        private IEnumerable<string> ArgumentsOnlyArgs()
        {
            return new List<string>
            {
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
