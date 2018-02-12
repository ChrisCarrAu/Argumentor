using System.Collections.Generic;

namespace ArgumentRes.Services.interfaces
{
    public interface IArgumentor<T>
    {
        /// <summary>
        /// Takes an array of strings - for example, from a command line 
        /// argument collection, and parses them to return a ParsedArguments 
        /// object.
        /// If the arguments passed in do no match those set up, an 
        /// ArgumentException is thrown.
        /// </summary>
        /// <param name="args">Arguments to parse</param>
        /// <returns>Parsed arguments</returns>
        T Parse(IEnumerable<string> args);

        /// <summary>
        /// Returns a Usage string which can be displayed to the console.
        /// </summary>
        /// <param name="command">The name of this executable - often App,</param>
        /// <returns></returns>
        string Usage(string command);
    }
}
