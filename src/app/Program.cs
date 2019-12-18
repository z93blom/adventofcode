﻿using AdventOfCode;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Loader;
using System.Text.RegularExpressions;

namespace AdventOfCode.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var usageProvider = new ApplicationUsage();

            var entryAssembly = Assembly.GetEntryAssembly();
            var files = Directory.EnumerateFiles(Path.GetDirectoryName(entryAssembly.Location), "solutions.dll");
            var assemblies = files.Select(f => AssemblyLoadContext.Default.LoadFromAssemblyPath(f));
            
            var tsolvers = assemblies.SelectMany(a => a.GetTypes())
                .Where(t => t.GetTypeInfo().IsClass && typeof(ISolver).IsAssignableFrom(t))
                .OrderBy(t => t.FullName)
                .ToArray();

            var action =
                Command(args, Args("update", "([0-9]+)/([0-9]+)"), m => {
                    var year = int.Parse(m[1]);
                    var day = int.Parse(m[2]);
                    return () => new Updater().Update(year, day, tsolvers, usageProvider).Wait();
                }) ??
                Command(args, Args("update", "last"), m => {
                    var dt = DateTime.Now;
                    if(dt.Month == 12 && dt.Day >=1 && dt.Day <= 25){
                        return () => new Updater().Update(dt.Year, dt.Day, tsolvers, usageProvider).Wait();
                    } else {
                        throw new Exception("Event is not active. This option works in Dec 1-25 only)");
                    }
                }) ??
                 Command(args, Args("([0-9]+)/([0-9]+)"), m => {
                    var year = int.Parse(m[0]);
                    var day = int.Parse(m[1]);
                    var tsolversSelected = tsolvers.First(tsolver => 
                        SolverExtensions.Year(tsolver) == year && 
                        SolverExtensions.Day(tsolver) == day);
                    return () => Runner.RunAll(tsolversSelected);
                }) ??
                 Command(args, Args("[0-9]+"), m => {
                    var year = int.Parse(m[0]);
                    var tsolversSelected = tsolvers.Where(tsolver => 
                        SolverExtensions.Year(tsolver) == year);
                    return () => Runner.RunAll(tsolversSelected.ToArray());
                }) ??
                Command(args, Args("([0-9]+)/last"), m => {
                    var year = int.Parse(m[0]);
                    var tsolversSelected = tsolvers.Last(tsolver =>
                        SolverExtensions.Year(tsolver) == year);
                    return () => Runner.RunAll(tsolversSelected);
                }) ??
                Command(args, Args("([0-9]+)/all"), m => {
                    var year = int.Parse(m[0]);
                    var tsolversSelected = tsolvers.Where(tsolver =>
                        SolverExtensions.Year(tsolver) == year);
                    return () => Runner.RunAll(tsolversSelected.ToArray());
                }) ??
                Command(args, Args("all"), m => {
                    return () => Runner.RunAll(tsolvers);
                }) ??
                Command(args, Args("last"),  m => {
                    var tsolversSelected = tsolvers.Last();
                    return () => Runner.RunAll(tsolversSelected);
                }) ??
                new Action(() => {
                    Console.WriteLine(usageProvider.Usage());
                });

            action();

        }

        static Action Command(string[] args, string[] regexes, Func<string[], Action> parse) {
            if (args.Length != regexes.Length) {
                return null;
            }
            var matches = Enumerable.Zip(args, regexes, (arg, regex) => new Regex("^" + regex + "$").Match(arg));
            if (!matches.All(match => match.Success)) {
                return null;
            }
            try {
                
                return parse(matches.SelectMany(m => m.Groups.Count > 1 ? m.Groups.Cast<Group>().Skip(1).Select(g => g.Value) : new []{m.Value}).ToArray());
            } catch {
                return null;
            }
        }

        static string[] Args(params string[] regex) {
            return regex;
        }

    }
}