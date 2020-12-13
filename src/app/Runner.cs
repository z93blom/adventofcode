using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace AdventOfCode
{

    public class Runner {

        public static void RunAll(params Type[] tsolvers) {
            var errors = new List<string>();

            var lastYear = -1;
            foreach (var solver in tsolvers.Select(tsolver => Activator.CreateInstance(tsolver) as ISolver)) {
                if (lastYear != solver.Year()) {
                    solver.SplashScreen().Show();
                    lastYear = solver.Year();
                }

                var workingDir = solver.WorkingDir();
                var color = Console.ForegroundColor;
                try {
                    WriteLine(ConsoleColor.White, $"{solver.DayName()}: {solver.GetName()}");
                    WriteLine();
                    foreach (var dir in new[] { workingDir, Path.Combine(workingDir, "test"), Path.Combine(@"..\..\..\..\..", workingDir) }) 
                    {
                        if (!Directory.Exists(dir)) {
                            continue;
                        }
                        var files = Directory.EnumerateFiles(dir).Where(file => file.EndsWith(".in")).ToArray();
                        foreach (var file in files) {

                            if (files.Count() > 1) {
                                WriteLine(color, "  " + file + ":");
                            }
                            var refoutFile = file.Replace(".in", ".refout");
                            var refout = File.Exists(refoutFile) ? File.ReadAllLines(refoutFile) : null;
                            var input = File.ReadAllText(file).TrimEnd();
                            var dt = DateTime.Now;
                            var iline = 0;
                            foreach (var line in solver.Solve(input)) {
                                var now = DateTime.Now;
                                var (statusColor, status, err) =
                                    refout == null || refout.Length <= iline ? (ConsoleColor.Cyan, "?", null) :
                                    refout[iline] == line.ToString() ? (ConsoleColor.DarkGreen, "✓", null) :
                                    (ConsoleColor.Red, "X", $"{solver.DayName()}: In line {iline + 1} expected '{refout[iline]}' but found '{line}'");

                                if (err != null) {
                                    errors.Add(err);
                                }

                                Write(statusColor, $"  {status}");
                                Write(color, $" {line} ");
                                var diff = (now - dt).TotalMilliseconds;
                                WriteLine(
                                    diff > 1000 ? ConsoleColor.Red :
                                    diff > 500 ? ConsoleColor.Yellow :
                                    ConsoleColor.DarkGreen,
                                    $"({diff} ms)"
                                );
                                dt = now;
                                iline++;
                            }
                        }
                    }

                    WriteLine();
                } finally {
                    Console.ForegroundColor = color;
                }
            }

            if (errors.Any()) {
                WriteLine(ConsoleColor.Red, "Errors:\n" + string.Join("\n", errors));
            }
        }

        private static void WriteLine(ConsoleColor color = ConsoleColor.Gray, string text = "") {
            Write(color, text + "\n");
        }
        private static void Write(ConsoleColor color = ConsoleColor.Gray, string text = "") {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }
    }
}