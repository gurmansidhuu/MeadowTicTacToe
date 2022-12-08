using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MeadowTicTacToe
{
    class Draw
    {
        readonly MicroGraphics graphics;

        public Draw(MicroGraphics graphics)
        {
            this.graphics = graphics;
        }

        public void MenuSetup()
        {

            graphics.Clear(true);
            graphics.CurrentFont = new Font12x16();

            //Draw Menu
            graphics.DrawText(55, 5, "Tic-Tac-Toe", Color.White);

            graphics.DrawRectangle
                (
                    x: 85,
                    y: 90,
                    width: 70,
                    height: 25,
                    color: Color.White);

            graphics.DrawText(90, 95, "Start", Color.Red);

            //Cursor
            graphics.DrawRectangle
                (
                    x: 80,
                    y: 85,
                    width: 80,
                    height: 35,
                    color: Color.Red);

            graphics.Show();
        }

        public void DrawCursor(int x, int y, int width, int height, Color color)
        {
            //Cursor
            graphics.DrawRectangle
                (
                    x: x,
                    y: y,
                    width: width,
                    height: height,
                    color: color
                );
        }

        public void Show()
        {
            graphics.Show();
        }


        public void GameSetup(string opp)
        {
            graphics.Clear();
            graphics.CurrentFont = new Font12x16();

            //Title
            graphics.DrawText(55, 5, "Tic-Tac-Toe", Color.White);

            //Draw grid
            graphics.DrawLine(89, 35, 89, 220, Color.GhostWhite);
            graphics.DrawLine(152, 35, 152, 220, Color.GhostWhite);

            graphics.DrawLine(25, 94, 215, 94, Color.GhostWhite);
            graphics.DrawLine(25, 157, 215, 157, Color.GhostWhite);

            //P1/P2 with colors
            graphics.DrawText(5, 5, "P1", Color.Red);
            graphics.DrawText(213, 5, opp, Color.Blue);

            graphics.Show();
        }

        public void DrawMoveX(int Position)
        {

            switch (Position)
            {
                case 0:
                    graphics.DrawLine(37, 42, 77, 82, Color.White);
                    graphics.DrawLine(37, 82, 77, 42, Color.White);
                    break;

                case 1:
                    graphics.DrawLine(102, 42, 142, 82, Color.White);
                    graphics.DrawLine(102, 82, 142, 42, Color.White);
                    break;

                case 2:
                    graphics.DrawLine(165, 42, 205, 82, Color.White);
                    graphics.DrawLine(165, 82, 205, 42, Color.White);
                    break;

                case 3:
                    graphics.DrawLine(37, 106, 77, 146, Color.White);
                    graphics.DrawLine(37, 146, 77, 106, Color.White);
                    break;

                case 4:
                    graphics.DrawLine(102, 106, 142, 146, Color.White);
                    graphics.DrawLine(102, 146, 142, 106, Color.White);
                    break;

                case 5:
                    graphics.DrawLine(165, 106, 205, 146, Color.White);
                    graphics.DrawLine(165, 146, 205, 106, Color.White);
                    break;

                case 6:
                    graphics.DrawLine(37, 170, 77, 210, Color.White);
                    graphics.DrawLine(37, 210, 77, 170, Color.White);
                    break;

                case 7:
                    graphics.DrawLine(102, 170, 142, 210, Color.White);
                    graphics.DrawLine(102, 210, 142, 170, Color.White);
                    break;

                case 8:
                    graphics.DrawLine(165, 170, 205, 210, Color.White);
                    graphics.DrawLine(165, 210, 205, 170, Color.White);
                    break;
                }

            graphics.Show();
        }

        public void DrawMoveO(int Position)
        {

            switch (Position)
            {
                case 0:
                    graphics.DrawCircle(57, 62, 21, Color.White);
                    break;

                case 1:
                    graphics.DrawCircle(122, 62, 21, Color.White);
                    break;

                case 2:
                    graphics.DrawCircle(185, 62, 21, Color.White);
                    break;

                case 3:
                    graphics.DrawCircle(57, 126, 21, Color.White);
                    break;

                case 4:
                    graphics.DrawCircle(122, 126, 21, Color.White);
                    break;

                case 5:
                    graphics.DrawCircle(185, 126, 21, Color.White);
                    break;

                case 6:
                    graphics.DrawCircle(57, 190, 21, Color.White);
                    break;

                case 7:
                    graphics.DrawCircle(122, 190, 21, Color.White);
                    break;

                case 8:
                    graphics.DrawCircle(185, 190, 21, Color.White);
                    break;
            }

            graphics.Show();
        }

        public void DisplayWinner(int winType, string winner, Color color)
        {
            graphics.CurrentFont = new Font12x20();

            //Print how game was won if not a draw
            if (winType != 8)
            {
                WinType(winType, color);

                //Print winner
                graphics.DrawRectangle(83, 75, 80, 55, Color.White, true);

                graphics.DrawRectangle(85, 77, 76, 51, color);

                graphics.DrawText(88, 80, "Winner", color);
                graphics.DrawText(112, 110, winner, color);
            }

            else
            {
                graphics.DrawRectangle(83, 75, 80, 55, Color.White, true);

                graphics.DrawRectangle(85, 77, 76, 51, color);

                graphics.DrawText(98, 90, "Draw", color);
            }

            //And option for new game or menu
            graphics.CurrentFont = new Font12x16();

            //Rematch
            graphics.DrawRectangle(75, 135, 94, 28, Color.White, true);
            graphics.DrawRectangle(77, 137, 90, 24, Color.Gray);

            graphics.DrawText(80, 140, "Rematch", Color.Gray);


            //Menu
            graphics.DrawRectangle(75, 165, 94, 28, Color.White, true);
            graphics.DrawRectangle(77, 167, 90, 24, Color.Gray);

            graphics.DrawText(98, 170, "Menu", Color.Gray);

            graphics.Show();

        }
        private void WinType(int winType, Color color)
        {
            //Draw how the game was won

            switch (winType)
            {
                /* 0 - 2 for horizontial */
                case 0:
                    graphics.DrawLine(32, 62, 210, 62, color);
                    break;

                case 1:
                    graphics.DrawLine(32, 126, 210, 126, color);
                    break;

                case 2:
                    graphics.DrawLine(32, 190, 210, 190, color);
                    break;

                /* 3 - 5 for vertical */
                case 3:
                    graphics.DrawLine(57, 37, 57, 215, color);
                    break;

                case 4:
                    graphics.DrawLine(122, 37, 122, 215, color);
                    break;

                case 5:
                    graphics.DrawLine(185, 37, 185, 215, color);
                    break;

                /* 6 & 7 for diagional */
                case 6:
                    graphics.DrawLine(32, 37, 210, 215, color);
                    break;

                case 7:
                    graphics.DrawLine(210, 37, 32, 215, color);
                    break;
            }
        }

        public void GameplayCursor(int Position, Color color)
        {

            switch (Position)
            {
                case 0:
                    DrawCursor(32, 37, 50, 50, color);
                    break;

                case 1:
                    DrawCursor(97, 37, 50, 50, color);
                    break;

                case 2:
                    DrawCursor(160, 37, 50, 50, color);
                    break;

                case 3:
                    DrawCursor(32, 101, 50, 50, color);
                    break;

                case 4:
                    DrawCursor(97, 101, 50, 50, color);
                    break;

                case 5:
                    DrawCursor(160, 101, 50, 50, color);
                    break;

                case 6:
                    DrawCursor(32, 165, 50, 50, color);
                    break;

                case 7:
                    DrawCursor(97, 165, 50, 50, color);
                    break;

                case 8:
                    DrawCursor(160, 165, 50, 50, color);
                    break;
            }
        }
    }
}
