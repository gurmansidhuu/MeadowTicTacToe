using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using System.Threading;

namespace MeadowTicTacToe
{
    class Game
    {
        readonly Draw draw;

        public int gamestate = 1;

        public int turn = 0;

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

        //Controlls the game with int gamestate
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

        //Main Menu - Gamestate 1
        private void Menu()
        {
            position = 0;

            draw.MenuSetup();

            //Wait for player 1 to start game
            do
            {
                AwaitInput();
            }
            while (gamestate == 1);

        }

        //Start TicTacToe - Gamestate 2
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

            do
            {
                //if all possible squares are filled declare draw
                if(turns > 8)
                {
                    winType = 8;
                    gamestate++;
                }

                //Alternate between P1 & P2 turns
                while (turn == 0)
                {
                    Player1Turn();
                }
                while(turn == 1)
                {
                    Player2Turn();
                }
            }
            while (gamestate == 2);
        }

        //Game Over overlayed ontop of game - Gamestate 3
        private void GameOver()
        {
            //Display winner or draw
            if(winType == 8)
            {
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

            //Player 1 chooses to rematch or return to main menu
            do
            {
                //Move cursor between the options and check for inputs
                if(position <= 0)
                {
                    position = 0;

                    draw.DrawCursor(77, 137, 90, 24, Color.Red);
                    draw.DrawCursor(77, 167, 90, 24, Color.Gray);
                }
                else
                {
                    position = 1;

                    draw.DrawCursor(77, 167, 90, 24, Color.Red);
                    draw.DrawCursor(77, 137, 90, 24, Color.Gray);
                }
                draw.Show();

                AwaitInput();
            }
            while (gamestate == 3);
        }

        //Player 1 Turn - Drawing cursor
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

        //Player 2 Turn - Drawing cursor
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

        //Inputs done with interrupt and bool values for singular vertical and horizontal movement
        public void AwaitInput()
        {
            vertIN = false;
            horzIN = false;

            Thread.Sleep(500);
        }

        
        //Checks if move so valid and not overiding a X/O
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

            //Check if game is won or swap move
            if (CheckWin(turn))
            {
                gamestate++;
            }
            else
            {
                turns++;
                position = 4;
            }
            
            return true;
        }

        //Check all possible scenarios of winning
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
            for(int i = 0; i < 3; i++)
            {
                if (Moves[4 * i] == (Turn + 1))
                {
                    checksum += Moves[4 * i];
                }
            }

            if (checksum == winCon)
            {
                winType = 6;
                return true;
            }
            else
            {
                checksum = 0;
            }

            //Diagonal Check - Top right to bottom left
            for (int i = 2; i < 7; i+= 2)
            {
                if (Moves[i] == (Turn + 1))
                {
                    checksum += Moves[i];
                }
            }

            if (checksum == winCon)
            {
                winType = 7;
                return true;
            }

            return false;
        }
    }
}
