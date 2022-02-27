using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Split_7
{
    class Program
    {
        private const int SIZE = 30;
        static string[] preset = { " ", "◊", "●" };

        static int turn = 0;

        static int cursor_x = 0;
        static int cursor_y = 0;

        private const int WIN_NUM = 7;

        static string[] logo =
        {
            "  ______           __ __   __            ________ ",
" /      \\         |  \\  \\ |  \\          |        \\",
"|  ▓▓▓▓▓▓\\ ______ | ▓▓\\▓▓_| ▓▓_          \\▓▓▓▓▓▓▓▓",
"| ▓▓___\\▓▓/      \\| ▓▓  \\   ▓▓ \\  ______    /  ▓▓ ",
" \\▓▓    \\|  ▓▓▓▓▓▓\\ ▓▓ ▓▓\\▓▓▓▓▓▓ |      \\  /  ▓▓  ",
" _\\▓▓▓▓▓▓\\ ▓▓  | ▓▓ ▓▓ ▓▓ | ▓▓ __ \\▓▓▓▓▓▓ /  ▓▓   ",
"|  \\__| ▓▓ ▓▓__/ ▓▓ ▓▓ ▓▓ | ▓▓|  \\       /  ▓▓    ",
" \\▓▓    ▓▓ ▓▓    ▓▓ ▓▓ ▓▓  \\▓▓  ▓▓      |  ▓▓     ",
"  \\▓▓▓▓▓▓| ▓▓▓▓▓▓▓ \\▓▓\\▓▓   \\▓▓▓▓        \\▓▓      ",
"         | ▓▓                                     ",
"         | ▓▓                                     ",
"          \\▓▓                                     ",
"",
"",
"",
"         Press [A] to start a new game",
"         Press [Q] to quit"
        };

        static string[] BlackWin =
        {
            " ======================== ",
            " Congratulations!         ",
            " Team Black won the game! ",
            " ------------------------ ",
            " Press [B] to go back     ",
            " Press [R] to restart     ",
            "                          "
        };

        static string[] WhiteWin =
        {
            " ======================== ",
            " Congratulations!         ",
            " Team White won the game! ",
            " ------------------------ ",
            " Press [B] to go back     ",
            " Press [R] to restart     ",
            "                          "
        };

        static string[] localGameHint =
        {
            "                                      ",
            " [Up/Down/Left/Right] Move the Cursor ",
            " [Enter] Place Your Chess             ",
            " [R] Restart                          ",
            " [B] Back To Menu                     ",
            "                                      "
        };

        static int[,] buffer = new int[SIZE, SIZE]; // buffer for chessboard, top, left

        static void Main(string[] args)
        {
            Init();

        // Main Screen
        MAIN_SCREEN:

            // Clear Screen
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            Clear();
            StartScreen();

            // options
            ConsoleKeyInfo mode = Console.ReadKey(true);
            if (mode.Key == ConsoleKey.A)
            {
                goto LOCAL_GAME;
            }
            else if (mode.Key == ConsoleKey.Q)
            {
                goto QUIT;
            }
            else goto MAIN_SCREEN;

            // Local Game
            LOCAL_GAME:

            // init
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;

            Console.Beep(880, 250);

            Clear();
            ClearChessBoard();

            UpdateInterface();

            while (true)
            {
                int handleRes = HandleKey();

                if (handleRes == 1)
                {
                    goto LOCAL_GAME;
                }
                if (handleRes == 2)
                {
                    goto MAIN_SCREEN;
                }

                UpdateInterface();

                int result = DetectWinner();

                if (result != -1)
                {
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.ForegroundColor = ConsoleColor.White;

                    if (result == 0)
                    {
                        PrintTextAt(70, 13, BlackWin);
                    }
                    else
                    {
                        PrintTextAt(70, 13, WhiteWin);
                    }

                    while (true)
                    {
                        ConsoleKey key = Console.ReadKey().Key;
                        if (key == ConsoleKey.B) goto MAIN_SCREEN;
                        if (key == ConsoleKey.R) goto LOCAL_GAME;
                    }
                }
            }

        QUIT:
            return;
        }

        static void Init()
        {
            Console.CursorVisible = false;
            Console.WindowHeight = 40;
            Console.WindowWidth = 120;
            Console.Title = "Split-7";
            Clear();
        }

        static void Clear()
        {
            Console.Clear();
        }

        static void StartScreen()
        {
            PrintTextAt(20, 1, logo);
            PrintTextAt(38, 11, "By Stehsaer");
        }

        static void PrintTextAt(int left, int top, string[] str)
        {
            int _top = top;
            foreach (string _str in str)
            {
                Console.SetCursorPosition(left, _top);
                Console.Write(_str);

                _top++;
            }
        }

        static void PrintTextAt(int left, int top, string str)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(str);
        }

        static void UpdateInterface()
        {
            Console.CursorVisible = false;
            UpdateCursor();
            UpdateChessBoard();
            UpdateInfo();
            //PrintTextAt(0, SIZE, String.Format("x={0} y={1}", cursor_x, cursor_y));
        }

        static void ClearChessBoard()
        {
            buffer = new int[SIZE, SIZE];
            cursor_x = 15;
            cursor_y = 15;
            turn = 0;
        }

        static void UpdateChessBoard()
        {

            for (int i = 0; i < SIZE; i++) // top
            {
                Console.SetCursorPosition(2, i + 1);

                for (int j = 0; j < SIZE; j++) // left
                {
                    if (cursor_x == j & cursor_y == i) goto NEXT;

                    int status = buffer[i, j];

                    if (status == 0)// blank
                    {
                        if ((i + j + 1) % 2 == 0) Console.BackgroundColor = ConsoleColor.DarkGray;
                        else Console.BackgroundColor = ConsoleColor.Gray;

                        Console.Write("  ");
                    }
                    else if (status == 1)// black
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write("[]");
                    }
                    else if (status == 2)// white
                    {
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.Write("[]");
                    }

                    goto JUMP;

                NEXT:
                    Console.SetCursorPosition(j * 2 + 4, i + 1);

                JUMP:
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                }
            }
        }

        static void UpdateCursor()
        {
            // print Cursor
            Console.SetCursorPosition(cursor_x * 2 + 2, cursor_y + 1);
            if (buffer[cursor_y, cursor_x] != 0)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            Console.Write("<>");
            Console.BackgroundColor = ConsoleColor.Black;
        }

        static void UpdateInfo()
        {
            Console.BackgroundColor = ConsoleColor.DarkYellow;
            Console.ForegroundColor = ConsoleColor.White;
            PrintTextAt(70, 1, string.Format(" Current Team: {0} ", turn == 0 ? "Black" : "White"));

            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;
            PrintTextAt(70, 3, string.Format(" Cursor X={00} ", cursor_x));
            PrintTextAt(70, 4, string.Format("        Y={00} ", cursor_y));

            Console.BackgroundColor = ConsoleColor.DarkCyan;
            Console.ForegroundColor = ConsoleColor.White;
            PrintTextAt(70, 6, localGameHint);
        }

        static int HandleKey()
        {
            ConsoleKey key = Console.ReadKey(true).Key;

            if (key == ConsoleKey.UpArrow)
            {
                if (cursor_y != 0)
                {
                    cursor_y -= 1;
                }
            }
            if (key == ConsoleKey.DownArrow)
            {
                if (cursor_y != SIZE - 1)
                {
                    cursor_y += 1;
                }
            }
            if (key == ConsoleKey.LeftArrow)
            {
                if (cursor_x != 0)
                {
                    cursor_x -= 1;
                }
            }
            if (key == ConsoleKey.RightArrow)
            {
                if (cursor_x != SIZE - 1)
                {
                    cursor_x += 1;
                }
            }
            if (key == ConsoleKey.R)
            {
                return 1;
            }
            if (key == ConsoleKey.B)
            {
                return 2;
            }
            if (key == ConsoleKey.Enter)
            {
                PlaceAt(cursor_x, cursor_y, turn);
            }

            return 0;
        }

        static void PlaceAt(int left, int top, int team)
        {
            // Clear Warning Message
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Black;
            PrintTextAt(20, 32, "                     ");

            if (buffer[top, left] != 0)
            {
                Console.BackgroundColor = ConsoleColor.Yellow;
                Console.ForegroundColor = ConsoleColor.Black;
                PrintTextAt(20, 32, " Can't Place It Here ");

                Console.Beep(550, 200);
                return;
            }

            if (team == 0) // black
            {
                for (int _top = top - 1; _top >= 0; _top--) // iterate upwards
                {
                    if (_top < 0) break;

                    if (buffer[_top, left] == 0) // turn black when met with blank
                        buffer[_top, left] = 1;

                    else if (buffer[_top, left] == 1) // turn blank when met with black
                    {
                        buffer[_top, left] = 0;
                        break;
                    }

                    else if (buffer[_top, left] == 2) // turn black when met with white
                    {
                        buffer[_top, left] = 1;
                        break;
                    }
                }

                for (int _top = top + 1; _top < SIZE; _top++) // iterate downwards
                {
                    if (_top >= SIZE) break;

                    if (buffer[_top, left] == 0) // turn black when met with blank
                        buffer[_top, left] = 1;

                    else if (buffer[_top, left] == 1) // turn blank when met with black
                    {
                        buffer[_top, left] = 0;
                        break;
                    }

                    else if (buffer[_top, left] == 2) // turn black when met with white
                    {
                        buffer[_top, left] = 1;
                        break;
                    }
                }
            }
            else if (team == 1)
            {
                for (int _left = left - 1; _left >= 0; _left--) // iterate left
                {
                    if (_left < 0) break;

                    if (buffer[top, _left] == 0)
                        buffer[top, _left] = 2;

                    else if (buffer[top, _left] == 1)
                    {
                        buffer[top, _left] = 2;
                        break;
                    }

                    else if (buffer[top, _left] == 2)
                    {
                        buffer[top, _left] = 0;
                        break;
                    }
                }

                for (int _left = left + 1; _left < SIZE; _left++) // iterate right
                {
                    if (_left >= SIZE) break;

                    if (buffer[top, _left] == 0)
                        buffer[top, _left] = 2;

                    else if (buffer[top, _left] == 1)
                    {
                        buffer[top, _left] = 2;
                        break;
                    }

                    else if (buffer[top, _left] == 2)
                    {
                        buffer[top, _left] = 0;
                        break;
                    }
                }
            }

            buffer[top, left] = team + 1;

            turn++;
            turn = turn % 2;

            Console.Beep(880, 200);
        }

        static int DetectWinner()
        {
            // vertical
            for (int left = 0; left < SIZE; left++)
            {
                int count = 0;
                for (int top = 0; top < SIZE; top++)
                {
                    int status = buffer[top, left];

                    if (status == 1) count++;
                    else
                    {
                        if (count == WIN_NUM)
                        {
                            for (int _top = top - 1; _top > top - 1 - WIN_NUM; _top--)
                            {
                                Console.BackgroundColor = ConsoleColor.Magenta;
                                Console.ForegroundColor = ConsoleColor.White;

                                PrintTextAt(left * 2 + 2, _top + 1, "::");
                            }
                            return 0;
                        }
                        else count = 0;
                    }
                }
            }

            // horizontal
            for (int top = 0; top < SIZE; top++)
            {
                int count = 0;
                for (int left = 0; left < SIZE; left++)
                {
                    int status = buffer[top, left];

                    if (status == 2) count++;
                    else
                    {
                        if (count == WIN_NUM)
                        {
                            for (int _left = left - 1; _left > left - 1 - WIN_NUM; _left--)
                            {
                                Console.BackgroundColor = ConsoleColor.Magenta;
                                Console.ForegroundColor = ConsoleColor.White;

                                PrintTextAt(_left * 2 + 2, top + 1, "::");
                            }
                            return 1;
                        }
                        else count = 0;
                    }
                }
            }

            return -1;
        }
    }
}
