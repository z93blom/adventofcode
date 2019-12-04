
using System;

namespace AdventOfCode.Y2018 {

    class SplashScreenImpl : AdventOfCode.SplashScreen {

        public override void Show() {

            var color = Console.ForegroundColor;
            Write(0xffff66, "\r\n  __   ____  _  _  ____  __ _  ____     __  ____     ___  __  ____  ____         \r\n / _\\ (    \\/ )");
            Write(0xffff66, "( \\(  __)(  ( \\(_  _)   /  \\(  __)   / __)/  \\(    \\(  __)        \r\n/    \\ ) D (\\ \\/ / ) _) /    /  ");
            Write(0xffff66, ")(    (  O )) _)   ( (__(  O )) D ( ) _)         \r\n\\_/\\_/(____/ \\__/ (____)\\_)__) (__)    \\__/(__)  ");
            Write(0xffff66, "   \\___)\\__/(____/(____)  2018\r\n\n                                                              ");
            Write(0x888888, "25 **\n                                                              24 **\n                          ");
            Write(0x888888, "                                    23 **\n                                                          ");
            Write(0x888888, "    22 **\n                                                              21 **\n                      ");
            Write(0x888888, "                                        20 **\n                                                      ");
            Write(0x888888, "        19 **\n                                                              18 **\n                  ");
            Write(0x888888, "                                            17 **\n                                                  ");
            Write(0x888888, "            16 **\n                                                              15 **\n              ");
            Write(0x888888, "                                                14 **\n                                              ");
            Write(0x888888, "                13 **\n                                                              12 **\n          ");
            Write(0x888888, "                                                    11 **\n                                          ");
            Write(0x888888, "                    10 **\n                                                               9 **\n      ");
            Write(0x888888, "             .---_                                       8 **\n                  / / /\\|             ");
            Write(0x888888, "                         7 **\n                / / | \\ *                                      6 **\n  ");
            Write(0x888888, "             /  /  \\ \\                                       5 **\n              / /  / \\  \\         ");
            Write(0x888888, "                             4 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           ");
            Write(0x888888, " ./~~~~~~~~~~~\\.                                    3 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           ");
            Write(0x888888, "( .\",^. -\". '.~ )                                   2 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           ");
            Write(0x888888, "_'~~~~~~~~~~~~~'_________ ___ __ _  _   _    _      1 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           \n");
            
            Console.ForegroundColor = color;
            Console.WriteLine();
        }
    }
}