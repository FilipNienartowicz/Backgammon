using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon
{
    public class Player
    {
        public String nick;
        public int score;
        public int color;

        public Player(String nick, int score, int color)
        {
            this.nick = nick;
            this.score = score;
            this.color = color;
        }

        public void UpScore(int i = 1)
        {
            this.score += i;
        }
    }
}
