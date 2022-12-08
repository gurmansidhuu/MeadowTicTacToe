using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Leds;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Foundation.Sensors.Hid;
using Meadow.Gateways.Bluetooth;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors.Hid;
using Meadow.Units;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MeadowTicTacToe
{
    class Game
    {
        readonly Draw draw;

        public int gamestate = 1;

        public int turn = 1;

        public int position = 0;

        private int turns = 0;
        private int winType = 8;

        public bool vertIN = false;
        public bool horzIN = false;

        private int[] Moves = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        public Game(MicroGraphics graphics)
        {
            draw = new Draw(graphics);
            
        }

        public void StartGame()
        { 
            while (true)
            {
                if (gamestate == 1)
                {
                    Menu();
                }

                else if (gamestate == 2)
                {
                    NewGame();
                }
                else if (gamestate == 3)
                {
                    GameOver();
                }
            }
            
        }

        private void Menu()
        {
            position = 0;

            draw.MenuSetup();

            do
            {
                AwaitInput();

            }
            while (gamestate == 1);

        }

        public void NewGame()
        {
            draw.GameSetup("P2");

            for (int i = 0; i < 9; i++)
            {
                Moves[i] = 0;
            }

            turns = 0;

            winType = 8;

            position = 4;

            turn = (turn + 1) % 2;

            do
            {
                if(turns > 8)
                {
                    winType = 8;
                    gamestate++;
                }

                if (turn == 0)
                {
                    Player1Turn();
                }

                else
                {
                    Player2Turn();
                }
            }
            while (gamestate == 2);

        }

        private void GameOver()
        {
            if(winType == 8)
            {
                //Draw
                draw.DisplayWinner(winType, "Draw", Color.Gray);
            }
            else if(turn == 1)
            {
                draw.DisplayWinner(winType, "P1", Color.Red);
            }
            else
            {
                draw.DisplayWinner(winType, "P2", Color.Blue);
            }

            do
            {
                if(position <= 0)
                {
                    draw.DrawCursor(77, 137, 90, 24, Color.Red);
                    draw.DrawCursor(77, 167, 90, 24, Color.Gray);
                }
                else
                {
                    draw.DrawCursor(77, 167, 90, 24, Color.Red);
                    draw.DrawCursor(77, 137, 90, 24, Color.Gray);
                }
                draw.Show();

                AwaitInput();
            }
            while (gamestate == 3);

        }

        private void Player1Turn()
        {
            AwaitInput();
            for (int i = 0; i < 9; i++)
            {
                draw.GameplayCursor(i, Color.Black);
            }
            draw.GameplayCursor(position, Color.Red);
            draw.Show();
        }

        private void Player2Turn()
        {
            AwaitInput();
            for(int i = 0; i < 9; i++)
            {
                draw.GameplayCursor(i, Color.Black);
            }
            draw.GameplayCursor(position, Color.Blue);
            draw.Show();
        }

        

        public void AwaitInput()
        {
            vertIN = false;
            horzIN = false;

            Thread.Sleep(500);
        }

        

        public bool CheckMove()
        {
            //if unvalid return
            if (Moves[position] != 0)
                return false;

            //if valid
            Moves[position] = turn + 1;

            if (turn == 0)
            {
                draw.DrawMoveX(position);
            }

            else
            {
                draw.DrawMoveO(position);
            }

            if (CheckWin(turn))
            {
                gamestate++;
            }
            else
            {
                
                position = 4;
            }
            
            return true;
        }

        private bool CheckWin(int Turn)
        {
            int winCon = (Turn + 1) * 3;

            int checksum = 0;

            //Horizontial Axis Check
            for (int i = 0; i < 7; i += 3)
            {
                for (int j = 0; j < 3; j++)
                {
                    if(Moves[j + i] == (Turn + 1))
                    {
                        checksum += Moves[j + i];
                    }
                }

                if (checksum == winCon)
                {
                    winType = i / 3;
                    return true;
                }
                else checksum = 0;
            }

            //Vertical Axis Check
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 7; j += 3)
                {
                    if (Moves[j + i] == (Turn + 1))
                    {
                        checksum += Moves[j + i];
                    }
                }

                if (checksum == winCon)
                {
                    winType = i + 3;
                    return true;
                }
                else checksum = 0;
            }

            //Diagonal Check - Top left to bottom right
            checksum = Moves[0] + Moves[4] + Moves[8];

            if (checksum == winCon)
            {
                winType = 6;
                return true;
            }

            //Diagonal Check - Top right to bottom left
            checksum = Moves[2] + Moves[4] + Moves[6];

            if (checksum == winCon)
            {
                winType = 7;
                return true;
            }

            return false;
        }
    }
}
