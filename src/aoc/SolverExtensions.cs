using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AdventOfCode
{
    public static class SolverExtensions {
        public static string DayName(this ISolver solver) {
            return $"Day {solver.Day()}";
        }

        public static int Year(this ISolver solver) {
            return Year(solver.GetType());
        }

        public static int Year(Type t) {
            return int.Parse(t.FullName.Split('.')[1].Substring(1));
        }
        public static int Day(this ISolver solver) {
            return Day(solver.GetType());
        }

        public static int Day(Type t) {
            return int.Parse(t.FullName.Split('.')[2].Substring(3));
        }

        public static string WorkingDir(int year) {
            return Path.Combine("src", "solutions", year.ToString());
        }

        public static string WorkingDir(int year, int day) {
            return Path.Combine(WorkingDir(year), "Day" + day.ToString("00"));
        }

        public static string WorkingDir(this ISolver solver) {
            return WorkingDir(solver.Year(), solver.Day());
        }

        public static SplashScreen SplashScreen(this ISolver solver) {
            // var tsplashScreen = Assembly.GetEntryAssembly().GetTypes()
            //      .Where(t => t.GetTypeInfo().IsClass && !t.IsAbstract && typeof(SplashScreen).IsAssignableFrom(t))
            //      .Single(t => Year(t) == solver.Year());
            var tsplashScreen = solver.GetType().Assembly.GetTypes()
                 .Where(t => t.GetTypeInfo().IsClass && !t.IsAbstract && typeof(SplashScreen).IsAssignableFrom(t))
                 .Single(t => Year(t) == solver.Year());
            return (SplashScreen)Activator.CreateInstance(tsplashScreen);
        }
    }
}