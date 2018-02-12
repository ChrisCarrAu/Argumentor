using ArgumentRes.Attributes;
using ArgumentRes.Services.implementations;
using System;
using System.Collections.Generic;

namespace ArgumentHandler
{
    internal class Program
    {
        internal class Arguments
        {
            [Switch(Key = "c", Description="Number of pings to send")]
            [Mandatory]
            public int Pings { get; set; }

            [Switch(Key = "t", Description = "Tengu Maru")]
            public string TenguMaru { get; set; }

            [Switch(Key = "x", Description = "no value")]
            [Mandatory]
            public bool IsSet { get; set; }

            [Switch(Key = "v", Description = "Victory Name")]
            public string VictoryName { get; set; }

            [Parameter(Key = "host", Description = "The name of the host")]
            public string Host { get; set; }

            [Parameter(Key = "...", Description = "Files to process")]
            public List<string> FilesToProcess { get; set; }
        }

        private static void Main(string[] args)
        {
            var argumentor = new Argumentor<Arguments>();
            /*
             * argumentor.AddSwitch("c", "The number of pings to send", Required.Mandatory);
            argumentor.AddSwitch("t", "Tengu maru", Required.Optional);
            argumentor.AddSwitch("u", "Umbrella count for those days when the rain doth pour in buckets and the hoi polloi do cower in corners and alleyways in fear of becoming wettened and soggy", Required.Optional);
            argumentor.AddSwitch("v", "Victor is an exceedingly handsome fellow", Required.Mandatory);
            argumentor.AddSwitch("w", "With enough help, anything is possible", Required.Optional);
            argumentor.AddSwitch("x", "With enough help, anything is possible", Required.Optional).HasValue = false;
            argumentor.AddArgument("host", "The name of the host", Required.Mandatory);
            argumentor.AddArguments("files to process", Required.Mandatory);
            */

            try
            {
                var commandLineArguments = argumentor.Parse(args);

                Console.WriteLine($"Pings: {commandLineArguments.Pings}");
            }
            catch (Exception e)
            {
                Console.Write("ERROR ");
                Console.WriteLine(e.Message);
                Console.WriteLine();
                Console.WriteLine(argumentor.Usage());
            }

            Console.ReadLine();

            return;
        }
    }
}
