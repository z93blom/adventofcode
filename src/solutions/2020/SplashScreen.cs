
using System;

namespace AdventOfCode.Y2020 {

    class SplashScreenImpl : AdventOfCode.SplashScreen {

        public override void Show() {

            var color = Console.ForegroundColor;
            Write(0xffff66, "\r\n  __   ____  _  _  ____  __ _  ____     __  ____     ___  __  ____  ____         \r\n / _\\ (    \\/ )");
            Write(0xffff66, "( \\(  __)(  ( \\(_  _)   /  \\(  __)   / __)/  \\(    \\(  __)        \r\n/    \\ ) D (\\ \\/ / ) _) /    /  ");
            Write(0xffff66, ")(    (  O )) _)   ( (__(  O )) D ( ) _)         \r\n\\_/\\_/(____/ \\__/ (____)\\_)__) (__)    \\__/(__)  ");
            Write(0xffff66, "   \\___)\\__/(____/(____)  2020\r\n\n                         ");
            Write(0x888888, "..........");
            Write(0xff0000, "|");
            Write(0x888888, "..........                 1 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n              ");
            Write(0x888888, ".....'''''' ");
            Write(0xcccccc, ".'  -  -  ");
            Write(0x888888, "\\");
            Write(0xcccccc, "- .''");
            Write(0x66ff, "~ ~ ");
            Write(0x888888, "''''''.....      2 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           ");
            Write(0x888888, "''' ");
            Write(0x66ff, "~ ~ ~ ~  ~ ");
            Write(0xcccccc, "'.'. -   - ");
            Write(0x888888, "\\ ");
            Write(0xcccccc, "-'':  ");
            Write(0x66ff, "~ ~   ~  ~  ");
            Write(0x888888, "'''   3 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n            ");
            Write(0x66ff, "~   ~  ~   ~ ~  ~ ");
            Write(0xcccccc, "''..'''");
            Write(0x888888, "_[]");
            Write(0xcccccc, ".'  ");
            Write(0x66ff, "~    ~   ~ ~  ~   ");
            Write(0x888888, " 4 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           ");
            Write(0x66ff, "~   ~ ~  ");
            Write(0x9900, ".'. ");
            Write(0x66ff, "~  ~  ~ ");
            Write(0x888888, "____/ ");
            Write(0xcccccc, "''  ");
            Write(0x66ff, "~  ~  ~     ~    ~  ");
            Write(0x888888, " 5 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n             ");
            Write(0x66ff, "~    ~ ");
            Write(0x9900, "''  ..");
            Write(0x888888, "_____/ ");
            Write(0x66ff, "~   ~  ~  ~      ~           ");
            Write(0x888888, " 6 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n            ");
            Write(0x66ff, "~  ~ ~   ~ ");
            Write(0x9900, ":");
            Write(0x888888, "[]");
            Write(0x9900, "'.   ");
            Write(0x66ff, "~   ~      ~                   ");
            Write(0x888888, " 7 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n                  ");
            Write(0x66ff, "~     ");
            Write(0x9900, "'.");
            Write(0x888888, "\\ ");
            Write(0x66ff, "~        ~  ~                     ");
            Write(0x888888, " 8 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n            ");
            Write(0x66ff, "~   ~      ~   ");
            Write(0x888888, "\\  ");
            Write(0x66ff, "~                             \n                          ~ ");
            Write(0x888888, "\\                                  9 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n                             ");
            Write(0x888888, "\\ ");
            Write(0x66ff, "~                            \n                              ");
            Write(0x888888, "\\                             \n                              ");
            Write(0x66ff, "~");
            Write(0x888888, "\\                            \n                                \\ ");
            Write(0x66ff, "~                         \n                              ~  ");
            Write(0x888888, "\\   ");
            Write(0x66ff, "~                        ");
            Write(0x888888, "10 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n                                  ");
            Write(0x888888, "\\   ");
            Write(0x9900, ".                     \n                                ");
            Write(0x66ff, "~  ");
            Write(0x888888, "\\");
            Write(0x9900, "'");
            Write(0x888888, "',");
            Write(0x9900, ":                    \n                                  :");
            Write(0x888888, "[].");
            Write(0x9900, ".'                      ");
            Write(0x888888, "11\n                                   '");
            Write(0x9900, "'     ");
            Write(0x66ff, "~                 \n           ");
            Write(0x888888, "                          ~                      \n                                                  ");
            Write(0x888888, "          \n                                           ");
            Write(0x66ff, "~");
            Write(0x888888, "              ~ \n                                      ");
            Write(0x66ff, "~                     \n                                        ");
            Write(0x9900, ". ..   .");
            Write(0x888888, "' ");
            Write(0x9900, ".");
            Write(0x888888, ".       ");
            Write(0x66ff, "~\n                                         ");
            Write(0x9900, "'.            ");
            Write(0x66ff, "~    \n           \n");
            
            Console.ForegroundColor = color;
            Console.WriteLine();
        }
    }
}