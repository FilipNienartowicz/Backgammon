using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Klasa reprezentujaca ruch gracza
namespace Backgammon.Classes
{
    //informacje, czy ruch jest podwojny, kosci oraz informacje o koncu tury
    public class Move
    {
        public bool x2move;
        private Dice[] dices;
        private bool endturn;
        public int color;

        public Move(int color, int dice1, int dice2)
        {
            x2move = false;
            if(dice1 == dice2 && dice1 + dice2 > 0)
            {
                x2move = true;
            }
            dices = new Dice[]{new Dice(dice1), new Dice(dice2)};
            endturn = false;
            this.color = color;
        }

        public Dice GetDice(int i)
        {
            if(i == 0 || i == 1)
            {
                return dices[i];
            }
            return null;
        }

        public Dice[] GetDices()
        {
            return dices;
        }

        public bool GetEndturn()
        {
            return endturn;
        }

        //Oznacza kosc o wartosci i jako uzyta
        public bool UseDice(int i)
        {
            if(!endturn)
            {
                if (dices[1].i == i && !dices[1].used)
                {
                    dices[1].UseDice();
                }
                else
                {
                    if (dices[0].i == i && !dices[0].used)
                    {
                        if (x2move == true)
                        {
                            if (!dices[1].used)
                            {
                                dices[1].UseDice();
                            }
                            else
                            {
                                dices[0].UseDice();
                                dices[1].UnUseDice();
                            }
                        }
                        else
                        {
                            dices[0].UseDice();
                        }
                    }
                }

                //Konczy ruch, gdy obie kosci wykorzystane
                if(dices[0].used && dices[1].used)
                {
                    if (!x2move)
                    {
                        endturn = true;
                        dices[0].i = 0;
                        dices[1].i = 0;

                    }
                    else
                    {
                        x2move = false;
                    }
                }
            }
            return endturn;
        }
    }
}
