using ArgumentRes.Models.interfaces;
using ArgumentRes.Services.implementations;
using System;

namespace ArgumentHandler
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var argumentor = new Argumentor();
            argumentor.AddSwitch("c", "The number of pings to send", Required.Mandatory);
            argumentor.AddSwitch("t", "Tengu maru", Required.Optional);
            argumentor.AddSwitch("u", "Umbrella count for those days when the rain doth pour in buckets and the hoi polloi do cower in corners and alleyways in fear of becoming wettened and soggy", Required.Optional);
            argumentor.AddSwitch("v", "Victor is an exceedingly handsome fellow", Required.Mandatory);
            argumentor.AddSwitch("w", "With enough help, anything is possible", Required.Optional);
            argumentor.AddSwitch("x", "With enough help, anything is possible", Required.Optional).HasValue = false;
            argumentor.AddArgument("host", "The name of the host", Required.Mandatory);
            argumentor.AddArguments("files to process", Required.Mandatory);

            try
            {
                var arguments = argumentor.Parse(args);

                foreach (var kvp in arguments.GetSwitches())
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }

                foreach (var kvp in arguments.GetParameters())
                {
                    Console.WriteLine($"{kvp.Key}: {kvp.Value}");
                }
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
