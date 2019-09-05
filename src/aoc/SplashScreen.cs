using System;

namespace AdventOfCode
{
    public abstract class SplashScreen {
        public abstract void Show();

        protected static void Write(int rgb, string text){
           Console.ForegroundColor = ClosestConsoleColor((rgb>>16)&255, (rgb>>8)&255, (rgb)&255);
           Console.Write(text);
       }

        protected static ConsoleColor ClosestConsoleColor(int r, int g, int b)
        {
            ConsoleColor ret = 0;
            double rr = r, gg = g, bb = b, delta = double.MaxValue;

            foreach (ConsoleColor cc in Enum.GetValues(typeof(ConsoleColor)))
            {
                var n = Enum.GetName(typeof(ConsoleColor), cc);
                var c = System.Drawing.Color.FromName(n == "DarkYellow" ? "Orange" : n); // bug fix
                var t = Math.Pow(c.R - rr, 2.0) + Math.Pow(c.G - gg, 2.0) + Math.Pow(c.B - bb, 2.0);
                if (t == 0.0)
                    return cc;
                if (t < delta)
                {
                    delta = t;
                    ret = cc;
                }
            }
            
            return ret;
        }
    }
}