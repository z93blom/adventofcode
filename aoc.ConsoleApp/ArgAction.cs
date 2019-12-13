using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace aoc.ConsoleApp
{
    internal class ArgAction
    {
        private readonly string[] _patterns;
        private readonly Func<string[], Func<Configuration, Task>> _action;
        private readonly Func<(string, string)> _usage;

        public ArgAction(string[] patterns, Func<string[], Func<Configuration, Task>> action, Func<(string, string)> usage)
        {
            _action = action;
            _usage = usage;
            _patterns = patterns;
        }

        public bool Match(string[] args)
        {
            if (args.Length != _patterns.Length)
            {
                return false;
            }

            var matches = args.Zip(_patterns, (arg, regex) => new Regex("^" + regex + "$").Match(arg));
            if (!matches.All(match => match.Success))
            {
                return false;
            }

            return true;
        }

        public Task Run(string[] args, Configuration config)
        {
            var matches = args.Zip(_patterns, (arg, regex) => new Regex("^" + regex + "$").Match(arg));
            var actionArguments = matches.SelectMany(m =>
                    m.Groups.Count > 1 ? m.Groups.Cast<Group>().Skip(1).Select(g => g.Value) : new[] {m.Value})
                .ToArray();

            return _action(actionArguments)(config);
        }

        public (string arguments, string description) Usage() => _usage();
    }
}