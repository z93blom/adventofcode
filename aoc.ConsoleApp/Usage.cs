using System;
using System.Collections.Generic;
using System.Linq;

namespace aoc.ConsoleApp
{
    internal class Usage
    {
        public static string Get(IEnumerable<ArgAction> actions)
        {
            var args = string.Join(Environment.NewLine, actions.Select(a =>
            {
                var (arg, desc) = a.Usage();
                return $"* {arg}: {desc}";
            }));

            return $@"
               > Usage: dotnet run [arguments]
               > Supported arguments:

{args}

               > To start working on new problems:
               > login to https://adventofcode.com, then copy your session cookie, and export it in your console like this 

               >   export SESSION=73a37e9a72a87b550ef58c590ae48a752eab56946fb7328d35857279912acaa5b32be73bf1d92186e4b250a15d9120a0

               > then run the app with

               >  update [year]/[day]   Prepares a folder for the given day, updates the input, 
               >                        the readme and creates a solution template.
               >  update last           Same as above, but for the current day. Works in December only.  
               > ".StripMargin("> ");
        }
    }
}