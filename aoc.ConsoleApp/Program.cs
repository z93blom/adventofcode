using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using aoc.Puzzles.Core;
using Fluent.IO;

namespace aoc.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var applicationStartupDirectory = new Path(Assembly.GetEntryAssembly().Location);
            while (!applicationStartupDirectory.IsDirectory)
            {
                applicationStartupDirectory = applicationStartupDirectory.Parent();
            }

            var configuration = Configuration.Load(applicationStartupDirectory);
            ISolutionHandler solutionHandler = new SolutionHandler();
            var actions = new List<ArgAction>();

            actions.Add(new ArgAction(new string[] { }, _ =>
            {
                return config =>
                {
                    Console.WriteLine(Usage.Get(actions.Skip(1)));
                    return Task.CompletedTask;
                };
            }, () => ("usage", "")));

            actions.Add(new ArgAction(new[] { "update", "([0-9]{4})[/-]([0-9][0-9]?)"}, m =>
            {
                var year = int.Parse(m[1]);
                var day = int.Parse(m[2]);

                return config => new Updater(config).Update(year, day);
            },() => ("update [year]/[day]", "Updates the puzzle project with data for the specified day.")));

            actions.Add(new ArgAction(new[] { "solve", "last" }, _ =>
                {
                    return async config =>
                    {
                        var moment = solutionHandler.Moments.OrderByDescending(m => m).First();
                        await solutionHandler.Solve(moment,
                            configuration.GetDir(moment),
                            Console.Out, 
                            CancellationToken.None);
                    };
                },
                () => ("solve last", "Solves the last day's puzzle.")));

            actions.Add(new ArgAction(new[] { "solve", "([0-9]{4})" }, matches =>
                {
                    var year = int.Parse(matches[1]);
                    return async config =>
                    {
                        var moments = solutionHandler.Moments.Where(m => m.Year == year).OrderBy(m => m);
                        foreach (var moment in moments)
                        {
                            await solutionHandler.Solve(moment,
                                configuration.GetDir(moment),
                                Console.Out,
                                CancellationToken.None);
                        }
                    };
                },
                () => ("solve [year]", "Solves all puzzles in the given year.")));


            var selected = actions.FirstOrDefault(aa => aa.Match(args)) ?? actions[0];
            await selected.Run(args, configuration);
        }

        static Action Command(string[] args, string[] regexes, Func<string[], Action> parse)
        {
            if (args.Length != regexes.Length)
            {
                return null;
            }
            var matches = Enumerable.Zip(args, regexes, (arg, regex) => new Regex("^" + regex + "$").Match(arg));
            if (!matches.All(match => match.Success))
            {
                return null;
            }
            try
            {

                return parse(matches.SelectMany(m => m.Groups.Count > 1 ? m.Groups.Cast<Group>().Skip(1).Select(g => g.Value) : new[] { m.Value }).ToArray());
            }
            catch
            {
                return null;
            }
        }

        static string[] Args(params string[] regex)
        {
            return regex;
        }
    }
}
