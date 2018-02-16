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
            [Flag(Key = "c", Description="Number of pings to send")]
            [Mandatory]
            public int Pings { get; set; }

            [Flag(Key = "t", Description = "Tengu Maru is the sea where Tengu comes from. Tengu is a penguin don't you know?")]
            [Mandatory]
            public string TenguMaru { get; set; }

            [Flag(Key = "x", Description = "no value")]
            [Mandatory]
            public bool IsSet { get; set; }

            [Flag(Key = "v", Description = "Victory Name")]
            public string VictoryName { get; set; }

            [Parameter(Key = "host", Description = "The name of the host")]
            public string Host { get; set; }

            [Parameter(Key = "...", Description = "Files to process")]
            public List<string> FilesToProcess { get; set; }
        }

        private static void Main(string[] args)
        {
            var argumentor = new Argumentor<Arguments>();

            try
            {
                var commandLineArguments = argumentor.Parse(args);

                Console.WriteLine($"Pings       : {commandLineArguments.Pings}");
                Console.WriteLine($"TenguMaru   : {commandLineArguments.TenguMaru}");
                Console.WriteLine($"IsSet       : {commandLineArguments.IsSet}");
                Console.WriteLine($"VictoryName : {commandLineArguments.VictoryName}");
                Console.WriteLine($"Host        : {commandLineArguments.Host}");
                foreach (var s in commandLineArguments.FilesToProcess)
                {
                    Console.WriteLine($"Argument    : {s}");
                }
            }
            catch (Exception e)
            {
                Console.Write("ERROR ");
                Console.WriteLine(e.Message);
                Console.WriteLine();
                Console.WriteLine(new UsageFormatter<Arguments>().ToString());
            }

            Console.ReadLine();

            return;
        }
    }
}
