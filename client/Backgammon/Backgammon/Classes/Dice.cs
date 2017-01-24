using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Classes
{
    public class Dice
    {
        public int i;
        public bool used;

        public Dice(int i)
        {
            this.i = i;
            this.used = false;
        }

        public void UseDice()
        {
            used = true;
        }

        public void UnUseDice()
        {
            used = false;
        }
    }
}
