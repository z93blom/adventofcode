
using System;

namespace AdventOfCode.Y2019 {

    class SplashScreenImpl : AdventOfCode.SplashScreen {

        public override void Show() {

            var color = Console.ForegroundColor;
            Write(0xffff66, "\r\n  __   ____  _  _  ____  __ _  ____     __  ____     ___  __  ____  ____         \r\n / _\\ (    \\/ )");
            Write(0xffff66, "( \\(  __)(  ( \\(_  _)   /  \\(  __)   / __)/  \\(    \\(  __)        \r\n/    \\ ) D (\\ \\/ / ) _) /    /  ");
            Write(0xffff66, ")(    (  O )) _)   ( (__(  O )) D ( ) _)         \r\n\\_/\\_/(____/ \\__/ (____)\\_)__) (__)    \\__/(__)  ");
            Write(0xffff66, "   \\___)\\__/(____/(____)  2019\r\n\n                            ");
            Write(0x888888, ". ''..     ':.                    25\n           .......            .   ''.   . '..                 2");
            Write(0x888888, "4\n                  '''''...  . .      ''.    '..               23\n              .           ''..   ");
            Write(0x888888, ".      '..   '.              22\n           ......             ''.         '.  . '. .          21\n   ");
            Write(0x888888, "              ''''...         '.  .   . .'.    :[.]     .  20 ");
            Write(0xffff66, "*");
            Write(0x888888, "*\n                        ''..       '.         '.   '.         19 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           ");
            Write(0x888888, ".....   .        ''.      '.        '..  [o]    .  18 **\n                ''''...      . '.      '.  ");
            Write(0x888888, "     ': . '.       17 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           ");
            Write(0x888888, "            '..       '.      '.       o    :      16 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           ");
            Write(0x888888, "               '.       '.     '.  .    : .  :     15 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           ");
            Write(0x888888, "'''''...      .  '.      '.    .(O)     .:   '.    14 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           ");
            Write(0x888888, "        ''..      .'.     '.  .  '. .    '.   :    13 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           ");
            Write(0x888888, "''''...  .  '.      '.     'O     '.      :    :   12 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           ");
            Write(0x888888, "       ''.    '.      :     '.     : .    '.   :   11 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           ");
            Write(0x888888, "'''''..   '..  '.      .     :.   .'.      :   '.  10 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n                 ");
            Write(0x888888, ".'.  '.   '.     '.     :     :      :    :   9 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n              ");
            Write(0x888888, ".     :  '.. ..      :     :     :       :   :   8 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           ");
            Write(0x888888, "'''.      :  : . :      :     :     :       :   :   7 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           ");
            Write(0x888888, ".  .      :  :   :      :     :     :       :   :   6 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           ");
            Write(0x888888, "...'      :  :   :      :  .  :     :       :   :   5 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n                ");
            Write(0x888888, ".   .  .'   :     .:     :  .  :       :   :   4 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           ");
            Write(0x888888, "       .'  .'   .'   . .'     :     :      :    :   3 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           ");
            Write(0x888888, ".....''   .'   .'      :     :     .'      :  ..'   2 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           ");
            Write(0x888888, "       ..'    .'      :.    .' .   :      .'   :    1 ");
            Write(0xfff66, "*");
            Write(0xffff66, "*\n           \n");
            
            Console.ForegroundColor = color;
            Console.WriteLine();
        }
    }
}