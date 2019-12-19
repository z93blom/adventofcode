using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Text;
using AdventOfCode.Utilities;
using AdventOfCode.Y2019.Day15;

namespace AdventOfCode.Y2019.Day19 {

    class Solution : ISolver {

        public string GetName() => "Tractor Beam";

        public IEnumerable<object> Solve(string input) {
            yield return PartOne(input);
            yield return PartTwo(input);
        }

        object PartOne(string input)
        {
            var affectedSquares = 0L;
            foreach (var y in Enumerable.Range(0, 50))
            {
                foreach (var x in Enumerable.Range(0, 50))
                {
                    var vm = new IntCode(input);
                    vm.ProvideInput(x);
                    vm.ProvideInput(y);
                    var result = vm.RunToNextInputOrFinishedCollectOutput();
                    affectedSquares += result[result.Length - 1];
                }
            }

            return affectedSquares;
        }

        object PartTwo(string input)
        {
            //// The beam is 8 wide at y = 50. When is it likely to be 100 wide (at least).
            //// it seems to be linear, so lets start at y = 100 * 50 / 8
            //var y = 100 * 50 / 8 - 100;
            //var startingY = 0;
            //var startingX = 0;
            //while (true)
            //{
            //    var x = y * 40 / 50;
            //    // Find a good y value to start at (by measuring the width of the beam at the location). We need to find the first place where the beam is 100 wide.
            //    var widthOfBeam = 0;

            //    while (!IsPulling(x++, y, input))
            //    {
            //    }

            //    startingX = x;
            //    while (IsPulling(x++, y, input))
            //    {
            //        widthOfBeam++;
            //    }

            //    if (widthOfBeam >= 100)
            //    {
            //        startingY = y;
            //        break;
            //    }

            //    y++;
            //}

            //return startingY; // 467, 569

            // Good place to start is at 467, 569 (the first place the beam is 100 wide)

            // Find a y where all of the corners are within the square.
            //var y = 600; //550;
            //while (true)
            //{
            //    var beamStart = BeamStart(input, y);
            //    var upperRight = BeamEnd(input, y, beamStart);
            //    var lowerLeft = new Point(upperRight - 100, y + 100);
            //    if (IsPulling(lowerLeft.X, lowerLeft.Y, input))
            //    {
            //        break;
            //    }

            //    y += 10;
            //}

            // y = 1040;
            //y -= 10;
            var y = 900;//600;
            while (y < 10000)
            {
                var beamStartTop = BeamStart(input, y);
                var beamEndTop = BeamEnd(input, y, beamStartTop);
                var beamStartBottom = BeamStart(input, y + 99);
                var beamEndBottom = BeamEnd(input, y + 99, beamStartBottom);

                if (beamStartBottom + 99 > beamEndTop)
                {
                    // No possible squares for this y-value.
                    y++;
                    continue;
                }


                // Theoretically there could be at least one 100*100 square that fits within the beam.
                for (var x = beamStartBottom; x + 99 <= beamEndTop; x++)
                {
                    if (Enumerable.Range(y, 100).All(yc => IsPulling(x, yc, input) && IsPulling(x + 99, yc, input)))
                    {
                        // All of the left and right borders fit within the beam. The top and bottom borders have also been checked.
                        return x * 10000 + y;
                    }
                }

                //var upperRight = new Point(BeamEnd(input, y, beamStart), y);
                //var lowerLeft = new Point(upperRight.X - 100, y + 100);
                //if (IsPulling(lowerLeft.X, lowerLeft.Y, input))
                //{
                //    // Candidate found. Do all the left and right borders work?
                //    if (Enumerable.Range(upperRight.Y, 100).All(yc => IsPulling(upperRight.X, yc, input) && IsPulling(lowerLeft.X, yc, input)))
                //    {
                //        return lowerLeft.X * 10000 + upperRight.Y;
                //    }
                //}

                y++;
            }

            return 0;

            // Now we are getting close at least.





            //var bw = BeamWidth(input, 569, out var startX);
            //var upperLeft = new Point(startX, 569);
            //var lowerRight = new Point(upperLeft.X + 100, upperLeft.Y + 100);


            //var bw100Lower = BeamWidth(input, lowerRight.Y, out var bStart);

            //// Find a good y-value where the corners are all pulling.



            //return 0;



            //foreach (var y in Enumerable.Range(100 * 50 / 8, 50))

            //{
            //    foreach (var x in Enumerable.Range(y * 40 / 50, 200))
            //    {
            //        var vm = new IntCode(input);
            //        vm.ProvideInput(x);
            //        vm.ProvideInput(y);
            //        var result = vm.RunToNextInputOrFinishedCollectOutput();
            //        sb.Append(result[result.Length - 1] == 1 ? '#': ' ');
            //    }

            //    sb.AppendLine();
            //}

            //Log(sb.ToString());

            //return 0;
        }

        bool IsPulling(int x, int y, string input)
        {
            var vm = new IntCode(input);
            vm.ProvideInput(x);
            vm.ProvideInput(y);
            var result = vm.RunToNextInputOrFinishedCollectOutput();
            return result[result.Length - 1] == 1;
        }

        int BeamStart(string input, int y)
        {
            var x = y * 40 / 50;

            while (!IsPulling(++x, y, input))
            {
            }

            return x;
        }

        int BeamEnd(string input, int y, int startX)
        {

            var x = startX - 1;
            while (IsPulling(++x, y, input))
            {
            }

            return x - 1;
        }

        int BeamWidth(string input, int y, out int startX)
        {
            var x = y * 40 / 50;
            var widthOfBeam = 0;

            while (!IsPulling(x++, y, input))
            {
            }

            startX = x;
            while (IsPulling(x++, y, input))
            {
                widthOfBeam++;
            }

            return widthOfBeam;
        }

        void Log(string text)
        {
            Console.Write(text);
        }
    }
}