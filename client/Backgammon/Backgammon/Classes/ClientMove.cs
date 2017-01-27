using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Klasa reprezentujaca ruch gracza (do przekzania w komunikacie)
namespace Backgammon.Classes
{
    public class ClientMove
    {
        public int from = 0;
        public int to = 0;
        public int endturn = 0;
    }
}
