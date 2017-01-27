using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Backgammon.Classes
{
    public class Game
    {
        public Board board;
        public Player player;
        public Player opponent;
        public Move opponentmove;
        public int[] startdices;
        public Move playermove;
        public int turn;

        public Game(string pl, int plsc, string opp, int oppsc, int plcol)
        {
            board = new Board();
            player = new Player(pl, plsc, plcol);
            opponent = new Player(opp, oppsc, 1 - plcol);
            opponentmove = new Move(1 - plcol, 0,0);
            playermove = new Move(plcol, 0, 0);

            startdices = new int[2];
            startdices[0] = 0;
            startdices[1] = 0;

            turn = 2;
        }

        public int GameStart()
        {
            if(startdices[0] != startdices[1])
            {
                if(startdices[0] > startdices[1])
                {
                    turn = player.color;
                    return 1;
                }
                else
                {
                    turn = opponent.color;
                    return 0;
                }
            }
            return 2;
        }

        public bool EndTurn()
        {
            turn = 1 - turn;
            if(turn == player.color)
            {
                return true;
            }
            else
            {
                playermove.dices[0].i = 0;
                playermove.dices[0].i = 1;
            }
            return false;
        }

        public void SetStartDices(int dice1, int dice2)
        {
            startdices[0] = dice1;
            startdices[1] = dice2;
        }

        public void SetOpponentDices(int dice1, int dice2)
        {
            opponentmove.dices[0] = new Dice(dice1);
            opponentmove.dices[1] = new Dice(dice2);
        }

        public void SetPlayerDices(int dice1, int dice2)
        {
            int color = playermove.color;
            playermove = new Move(color, dice1, dice2);
        }

        public int SelectField(int x, int y, Classes.ClientMove clientmove)
        {
            if(turn == player.color)
            {
                int move = board.TrytoSelectField(player, playermove, x, y, clientmove);
                if(move == 2)
                {
                    turn = 1 - turn;
                }
                return move;
            }
            return 0;
        }
    }
}
