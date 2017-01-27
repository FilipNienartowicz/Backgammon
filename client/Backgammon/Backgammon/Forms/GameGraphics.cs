using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Windows.Forms;

namespace Backgammon
{
    //Klasa odpowiedzialna za wyswietlanie gry na planszy
    class GameGraphics
    {
        private Bitmap board = null;
        private Bitmap BoardImage;
        private Bitmap[,] DiceImage;
        private Bitmap[] PawnImage;
        private Bitmap[] MarkerImage;
        private int[] pawnsdistance;

        public GameGraphics()
        {
            try{
                BoardImage = new Bitmap((Bitmap)Image.FromFile(@"Grafika\Plansza.png", true), new Size(840, 860));

                board = new Bitmap((Bitmap)Image.FromFile(@"Grafika\Plansza.png", true), new Size(840, 860));

                DiceImage = new Bitmap[2,7];
                for (int i = 1; i < 7; i++)
                {
                    DiceImage[0,i] = (Bitmap) Image.FromFile(@"Grafika\Kostkaw" + i.ToString() + ".png", true);
                }
                
                for(int i = 1; i < 7; i++)
                {
                    DiceImage[1,i] = (Bitmap) Image.FromFile(@"Grafika\Kostkar" + i.ToString() + ".png", true);
                }

                PawnImage = new Bitmap[2];
                PawnImage[0] = new Bitmap((Bitmap) Image.FromFile(@"Grafika\Pionekw.png", true), new Size(50,50)); 
                PawnImage[1] = new Bitmap((Bitmap) Image.FromFile(@"Grafika\Pionekr.png", true), new Size(50,50));
                
                MarkerImage = new Bitmap[2];
                MarkerImage[0] = (Bitmap) Image.FromFile(@"Grafika\Znacznikpelny.png", true);
                MarkerImage[1] = (Bitmap) Image.FromFile(@"Grafika\Znacznikpusty.png", true);
            }
            catch(System.IO.FileNotFoundException)
            {
                MessageBox.Show("There was an error opening the graphics. Make sure there is a folder 'Grafika' next to backgammon.exe");
                Application.Exit();
            }

            pawnsdistance = new int[16];
            pawnsdistance[0] = 0;
            for(int i=1; i < 16; i++)
            {
                pawnsdistance[i] = Math.Min(45, (int)(315/i));
            }
        }

        //Przygotowuje grafike planszy
        public Bitmap CreateGameBoard(Classes.Game game)
        {
            Graphics g = Graphics.FromImage(board);
   
            g.DrawImage(BoardImage, new Point(0,0));
            if (game != null)
            {
                //wyswietla pionki i zaznaczone pole
                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < game.board.table[i].pawns; j++)
                    {
                        g.DrawImage(PawnImage[game.board.table[i].color], new Point(727 - i * 55, 30 + pawnsdistance[game.board.table[i].pawns] * j));   
                    }
                    
                    if(game.board.selected == i)
                    {
                        g.DrawImage(MarkerImage[0], new Point(750 - i * 55, 10));
                    }

                    if (game.board.possiblemoves[0] == i || game.board.possiblemoves[1] == i)
                    {
                        g.DrawImage(MarkerImage[1], new Point(750 - i * 55, 10));
                    }
                }

                for (int i = 6; i < 12; i++)
                {
                    for (int j = 0; j < game.board.table[i].pawns; j++)
                    {
                        g.DrawImage(PawnImage[game.board.table[i].color], new Point(332 - (i - 6) * 55, 30 + pawnsdistance[game.board.table[i].pawns] * j));
                    }

                    if (game.board.selected == i)
                    {
                        g.DrawImage(MarkerImage[0], new Point(355 - (i - 6) * 55, 10));
                    }

                    if (game.board.possiblemoves[0] == i || game.board.possiblemoves[1] == i)
                    {
                        g.DrawImage(MarkerImage[1], new Point(355 - (i - 6) * 55, 10));
                    }
                }

                for (int i = 12; i < 18; i++)
                {
                    for(int j = 0; j < game.board.table[i].pawns; j++)
                    {
                        g.DrawImage(PawnImage[game.board.table[i].color], new Point(57 + (i - 12) * 55, 780 - pawnsdistance[game.board.table[i].pawns] * j)); 
                    }

                    if (game.board.selected == i)
                    {
                        g.DrawImage(MarkerImage[0], new Point(80 + (i - 12) * 55, 835));
                    }

                    if (game.board.possiblemoves[0] == i || game.board.possiblemoves[1] == i)
                    {
                        g.DrawImage(MarkerImage[1], new Point(80 + (i - 12) * 55, 835));
                    }
                }

                for (int i = 18; i < 24; i++)
                {
                    for (int j = 0; j < game.board.table[i].pawns; j++)
                    {
                        g.DrawImage(PawnImage[game.board.table[i].color], new Point(457 + (i - 18) * 55, 780 - pawnsdistance[game.board.table[i].pawns] * j));
                    }

                    if (game.board.selected == i)
                    {
                        g.DrawImage(MarkerImage[0], new Point(480 + (i - 18) * 55, 835));
                    }

                    if (game.board.possiblemoves[0] == i || game.board.possiblemoves[1] == i)
                    {
                        g.DrawImage(MarkerImage[1], new Point(480 + (i - 18) * 55, 835));
                    }
                }

                for (int j = 0; j < game.board.table[24].pawns; j++)
                {
                    g.DrawImage(PawnImage[game.board.table[24].color], new Point(395, 200 + pawnsdistance[game.board.table[24].pawns] * j));
                    if (game.board.selected == 24)
                    {
                        g.DrawImage(MarkerImage[0], new Point(415, 10));
                    }
                }

                for (int j = 0; j < game.board.table[25].pawns; j++)
                {

                    g.DrawImage(PawnImage[game.board.table[25].color], new Point(400, 600 - pawnsdistance[game.board.table[25].pawns] * j));
                    if (game.board.selected == 25)
                    {
                        g.DrawImage(MarkerImage[0], new Point(420, 835));
                    }   
                }

                for (int j = 0; j < game.board.table[26].pawns; j++)
                {
                    g.DrawImage(PawnImage[game.board.table[26].color], new Point(790, 700 - 10 * j));   
                }

                if (game.board.possiblemoves[0] == 26 || game.board.possiblemoves[1] == 26)
                {
                    g.DrawImage(MarkerImage[1], new Point(808, 760));
                }

                for (int j = 0; j < game.board.table[27].pawns; j++)
                {
                    g.DrawImage(PawnImage[game.board.table[27].color], new Point(790, 100 + 10 * j));
                    
                }

                if (game.board.possiblemoves[0] == 27 || game.board.possiblemoves[1] == 27)
                {
                    g.DrawImage(MarkerImage[1], new Point(808, 70));
                }

                //wyswietla kosci
                if(game.turn == 0)
                {
                    Classes.Dice[] dices = null;
                    if (game.playermove != null && !game.playermove.GetEndturn())
                    {
                        if(game.playermove.color == 0)
                        {
                            dices = game.playermove.GetDices();
                        }
                    }
                    
                    if (game.opponentmove != null)
                    {
                        
                        if(game.opponentmove.color == 0)
                        {
                            dices = game.opponentmove.GetDices();
                        }
                    }

                    if( dices != null)
                    {
                        if (dices[0].i > 0)
                        {
                            if (dices[0].used)
                            {
                                g.DrawImage(new Bitmap(DiceImage[0, dices[0].i], new Size(40, 40)), new Point(180, 410));
                            }
                            else
                            {
                                g.DrawImage(DiceImage[0, dices[0].i], new Point(170, 400));
                            }
                        }

                        if (dices[0].i > 0)
                        {
                            if (dices[1].used)
                            {
                                g.DrawImage(new Bitmap(DiceImage[0, dices[1].i], new Size(40, 40)), new Point(260, 410));
                            }
                            else
                            {
                                g.DrawImage(DiceImage[0, dices[1].i], new Point(250, 400));
                            }
                        }   
                    }           
                }

                if (game.turn == 1)
                {
                    Classes.Dice[] dices = null;
                    if (game.playermove != null)
                    {

                        if (game.playermove.color == 1)
                        {
                            dices = game.playermove.GetDices();
                        }
                    }

                    if (game.opponentmove != null)
                    {

                        if (game.opponentmove.color == 1)
                        {
                            dices = game.opponentmove.GetDices();
                        }
                    }

                    if (dices != null)
                    {
                        if (dices[0].i > 0)
                        {
                            if (dices[0].used)
                            {
                                g.DrawImage(new Bitmap(DiceImage[1, dices[0].i], new Size(40, 40)), new Point(530, 410));
                            }
                            else
                            {
                                g.DrawImage(DiceImage[1, dices[0].i], new Point(520, 400));
                            }
                        }

                        if (dices[1].i > 0)
                        {
                            if (dices[1].used)
                            {
                                g.DrawImage(new Bitmap(DiceImage[1, dices[1].i], new Size(40, 40)), new Point(610, 410));
                            }
                            else
                            {
                                g.DrawImage(DiceImage[1, dices[1].i], new Point(600, 400));
                            }
                        }
                    }    
                }

                if (game.turn == 2)
                {
                    if (game.GetStartDices()[game.player.color] > 0)
                    {
                        g.DrawImage(DiceImage[0, game.GetStartDices()[game.player.color]], new Point(250, 400));
                    }
                    if (game.GetStartDices()[1 - game.player.color] > 0)
                    {
                        g.DrawImage(DiceImage[1, game.GetStartDices()[1 - game.player.color]], new Point(550, 400));
                    }
                }
            }
            return board;
        }
    }
}
