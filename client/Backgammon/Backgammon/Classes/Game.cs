using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Klasa reprezentujaca gre
namespace Backgammon.Classes
{
    public class Game
    {
        public Board board;
        public Player player;
        public Player opponent;
        public Move opponentmove;
        private int[] startdices;
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

        //Rozpoczyna gre. Zwraca kolor zaczynajacego
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
            turn = 2;
            return 2;
        }

        //Konczy ture (zmienia turn na kolor drugiego gracza) zwraca true, jesli tura gracza
        public bool EndTurn()
        {
            turn = 1 - turn;
            if(turn == player.color)
            {
                return true;
            }
            return false;
        }

        public int[] GetStartDices()
        {
            return startdices;
        }

        public void SetStartDices(int dice1, int dice2)
        {
            startdices[0] = dice1;
            startdices[1] = dice2;
        }

        //Ustawia kosci przeciwnika, jesli nie ma mozliwych ruchow zglasza to, aby zakonczyc ture
        public bool SetOpponentDices(int dice1, int dice2)
        {
            int color = opponentmove.color;
            opponentmove = new Move(color, dice1, dice2);
            if (dice1 > 0 && dice2 > 0 && !board.isAnyPossibleMove(opponentmove))
            {
                return false;
            }
            return true;
        }

        //Ustawia kosci gracza, jesli nie ma mozliwych ruchow zglasza to, aby zakonczyc ture
        public bool SetPlayerDices(int dice1, int dice2)
        {
            int color = playermove.color;
            playermove = new Move(color, dice1, dice2);
            if (dice1 > 0 && dice2 > 0 && !board.isAnyPossibleMove(playermove))
            {
                return false;
            }
            return true;
        }

        //Reakcja na klikniecie pola gry - dopasowuje klikniecie, wybiera pole i robi ruch. Jesli ruch konczyl ture, to zmienia ture.
        public int SelectField(int x, int y, Classes.ClientMove clientmove)
        {
            if(turn == player.color)
            {
                int move = board.TrytoSelectField(player, playermove, x, y, clientmove);
                if(move >= 2)
                {
                    turn = 1 - turn;

                    if(move == 4)
                    {
                        turn = 2;
                        startdices[0] = 0;
                        startdices[1] = 0;
                    }
                }
                return move;
            }
            return 0;
        }
    }
}
