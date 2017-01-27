using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Klasa reprezentujaca plansze do gry
namespace Backgammon.Classes
{
    public class Board
    {
        public Field[] table; // 0 - 23 - pola gry; 24 i 25 - zbite; 26 i 27 - w domach
        public int[] taken; //adres zbitych
        public int[] home; //adres domu
        public int selected; //zaznaczone pole
        public int [] possiblemoves; //mozliwe ruchy z tego pola
        public int from;
        public int to;
        
        public Board()
        {
            taken = new int[2];
            taken[0] = 24;
            taken[1] = 25;

            home = new int[2];
            home[0] = 26;
            home[1] = 27;
            
            table = new Field[28];
            for (int i = 0; i < 28; i++)
            {
                table[i] = new Field();
            }

            //rozstawia pionki na planszy
            table[0].SetFieldValues(2, 0);
            table[5].SetFieldValues(5, 1);
            table[7].SetFieldValues(3, 1);
            table[11].SetFieldValues(5, 0);
            table[12].SetFieldValues(5, 1);
            table[16].SetFieldValues(3, 0);
            table[18].SetFieldValues(5, 0);
            table[23].SetFieldValues(2, 1);

            table[24].color = 0;
            table[25].color = 1;
            table[26].color = 0;
            table[27].color = 1;

            selected = -1;
            possiblemoves = new int[2];
            possiblemoves[0] = -1;
            possiblemoves[1] = -1;
        }

        /*sprawdza, czy pola from i to sa mniejsze/wieksze od siebie (odpowiednio dla koloru) 
         * sprawdza tylko czy istnieje taki ruch, nie sprawdza poprawnosci jego wykonania
        */
        public bool CanMoveThatWay(int color, int from, int to)
        {
            if(from < 24 && to < 24 )
            {
                if(color == 0 && from < to)
                {
                    return true;
                }

                if (color == 1 && from > to)
                {
                    return true;
                }
            }
            else
            {
                if(to == home[color])
                {
                    return true;
                }

                if (from == taken[color])
                {
                    return true;
                }
            }
            return false;
        }

        //Przenosi pionek miedzy polami (jesli to mozliwe)
        public int Move(Player player, Move move, int from, int to)
        {
            if (CanMoveThatWay(player.color, from, to))
            {
                //jesli true, to zostanie wykonany ruch
                bool canmove = false;
                //jaka kosc ma zostac uzyta
                int dicevalue = 0;

                if (from < 24 && to < 24)
                {
                    //Przenoszenie miedzy zwyklymi polami          
                    
                    //mozna uzyc kosci 0
                    if (move.GetDice(0).i == Math.Abs(from - to) && !move.GetDice(0).used)
                    {
                        canmove = true;
                        dicevalue = move.GetDice(0).i;
                    }
                    
                    //mozna uzyc kosci 1
                    if (move.GetDice(1).i == Math.Abs(from - to) && !move.GetDice(1).used)
                    {
                        canmove = true;
                        dicevalue = move.GetDice(1).i;
                    }
                        
                    //kosci rowne sb, mozna uzyc
                    if (move.GetDice(1).i == Math.Abs(from - to) && move.GetDice(0).i == move.GetDice(1).i)
                    {   
                        canmove = true;
                        dicevalue = move.GetDice(0).i;
                    }  
                }
                else
                {
                    //Wprowadzanie do domu przeciwnika zbitych pionkow gracza
                    if(from == taken[move.color] && table[taken[move.color]].pawns > 0)
                    {
                        //liczba oczek potrzebna na kosci
                        int movevalue = to;
                        if (move.color == 0)
                        {
                            movevalue = to + 1;
                        }
                        else
                        {
                            movevalue = 24 - to;
                        }

                        //mozna uzyc kosci 0
                        if (move.GetDice(0).i == movevalue && !move.GetDice(0).used)
                        {
                            canmove = true;
                            dicevalue = move.GetDice(0).i;
                        }

                        //mozna uzyc kosci 1
                        if (move.GetDice(1).i == movevalue && !move.GetDice(1).used)
                        {
                            canmove = true;
                            dicevalue = move.GetDice(1).i;
                        }

                        //kosci rowne sb, mozna uzyc
                        if (move.GetDice(1).i == movevalue && move.GetDice(0).i == move.GetDice(1).i)
                        {
                            canmove = true;
                            dicevalue = move.GetDice(0).i;
                        }  
                    }

                    //zdejmowanie pionkow z planszy - wybrany dom gracza i wszystkie pionki w domu
                    if (to == home[move.color] && IsEnding(move.color))
                    {
                        //liczba oczek potrzebna na kosci - w przypadku konczacych ruchow mozna uzyc kosci wiekszej; warunkiem dodatkowym - wszystkie pola dalej od domu, niz przenoszony musza byc puste
                        int movevalue = from + 1;
                        if(move.color == 0)
                        {
                            movevalue = 24 - from;
                        }

                        //mozna uzyc kosci 0 - kosc rowna odleglosci
                        if (move.GetDice(0).i == movevalue && !move.GetDice(0).used)
                        {
                            canmove = true;
                            dicevalue = move.GetDice(0).i;
                        }

                        //mozna uzyc kosci 1 - kosc rowna odleglosci
                        if (move.GetDice(1).i == movevalue && !move.GetDice(1).used)
                        {
                            canmove = true;
                            dicevalue = move.GetDice(1).i;
                        }

                        //kosci rowne sb, mozna uzyc - kosc rowna odleglosci
                        if (move.GetDice(1).i == movevalue && move.GetDice(0).i == move.GetDice(1).i)
                        {
                            canmove = true;
                            dicevalue = move.GetDice(0).i;
                        }

                        //Kosc wieksza niz odleglosc (nadal nie ma ruchu, nie ma bardziej odleglych)
                        if (!canmove && !isPawnInHomeFurther(move.color, from))
                        {
                            //mozna uzyc kosci 0 - kosc rowna odleglosci
                            if (move.GetDice(0).i > movevalue && !move.GetDice(0).used)
                            {
                                canmove = true;
                                dicevalue = move.GetDice(0).i;
                            }

                            //mozna uzyc kosci 1 - kosc rowna odleglosci
                            if (move.GetDice(1).i > movevalue && !move.GetDice(1).used)
                            {
                                canmove = true;
                                dicevalue = move.GetDice(1).i;
                            }

                            //kosci rowne sb, mozna uzyc - kosc rowna odleglosci
                            if (move.GetDice(1).i > movevalue && move.GetDice(0).i == move.GetDice(1).i)
                            {
                                canmove = true;
                                dicevalue = move.GetDice(0).i;
                            }
                        }
                    }
                }

                //Jesli oznaczono jakis ruch jako mozliwy, to wykonuje go
                if (canmove)
                {
                    int dec = table[to].AddPawn(player.color);
                    if (dec > 0)
                    {
                        table[from].RemovePawn();
                        if (dec == 2)
                        {
                            table[taken[1 - player.color]].AddPawn(1 - player.color);
                        }

                        bool endgame = false;
                        if(table[home[player.color]].pawns == 15)
                        {
                            endgame = true;
                        }

                        if (move.UseDice(dicevalue))
                        {
                            if(endgame)
                            {
                                return 4;
                            }
                            return 2;
                        }

                        if (endgame)
                        {
                            return 4;
                        }
                        return 1;
                    }
                }
            }
            return 0;
        }

        //klasa pola planszy
        public class Field
        {
            public int pawns = 0;
            public int color = 2; //0 - white, 1 - red, 2 - no color

            //Ustawia pola obiektu field
            public void SetFieldValues(int pawns, int color)
            {
                this.pawns = pawns;
                this.color = color;
            }

            //Zmniejsza liczbe pionkow na polu
            public void RemovePawn()
            {
                if (pawns > 0)
                {
                    pawns--;
                    if (pawns == 0)
                        color = 2;
                }
            }

            //Dodaje pionek do pola (jesli to mozliwe)
            //0 - not possible, 1 - ok, 2 - opponent pawn taken
            public int AddPawn(int color)
            {
                if (this.color == color)
                {
                    pawns++;
                    return 1;
                }

                if (this.color == 2)
                {
                    SetFieldValues(1, color);
                    return 1;
                }

                if (this.pawns <= 1)
                {
                    bool take = false;
                    if (this.pawns == 1)
                        take = true;
                    SetFieldValues(1, color);

                    if (take)
                    {
                        return 2;
                    }
                    return 1;
                }
                return 0;
            }
        }

        //Wypelnia tablice mozliwymi ruchami z zaznaczonego pola
        public void PossibleMoves(int selected, Move move, int []possiblemoves)
        {
            possiblemoves[0] = -1;
            possiblemoves[1] = -1;
            if(selected >= 0 && table[selected].color == move.color)
            {
                //wybrane pole jest zwyklym polem na planszy
                if (selected < 24)
                {
                    int mn = -1;
                    if (move.color == 0)
                    {
                        mn = 1;
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        int pos = selected + mn * move.GetDice(i).i;
                        if (pos > 23 || pos < 0)
                        {
                            //Przesuniecie do domu
                            if (IsEnding(move.color))
                            {
                                if (move.GetDice(0).i == move.GetDice(1).i || move.GetDice(i).used == false)
                                {
                                    if (pos == -1 || pos == 24)
                                    {
                                        possiblemoves[i] = home[move.color];
                                    }

                                    if (!isPawnInHomeFurther(move.color, selected))
                                    {
                                        possiblemoves[i] = home[move.color];
                                    }
                                }
                            }
                        }
                        else
                        {
                            //Przesuniecie na inne pole
                            if (CanSelect(move.color, pos))
                            {
                                if (move.GetDice(0).i == move.GetDice(1).i || move.GetDice(i).used == false)
                                {
                                    possiblemoves[i] = pos;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if(selected == taken[move.color])
                    {
                        if(move.color == 0)
                        {
                            for (int i = 0; i < 2; i++)
                                if (CanSelect(move.color, move.GetDice(i).i - 1))
                                {
                                    if (move.GetDice(0).i == move.GetDice(1).i || move.GetDice(i).used == false)
                                    {
                                        possiblemoves[i] = move.GetDice(i).i - 1;
                                    }
                                }
                        }
                        else
                        {
                            for (int i = 0; i < 2; i++)
                                if (CanSelect(move.color, 24 - move.GetDice(i).i))
                                {
                                    if (move.GetDice(0).i == move.GetDice(1).i || move.GetDice(i).used == false)
                                    {
                                        possiblemoves[i] = 24 - move.GetDice(i).i;
                                    }
                                }
                        }
                    }
                }
            }
        }

        //Sprawdza, czy podane pole moze zostac zaznaczone przez gracza
        private bool CanSelect(int color, int selected)
        {
            if(selected >= 0)
            {
                if (selected < 24)
                {
                    if (table[selected].color == 2)
                    {
                        return true;
                    }

                    if (table[selected].color == color)
                    {
                        return true;
                    }

                    if (table[selected].color != color && table[selected].pawns <= 1)
                    {
                        return true;
                    }
                }
                else
                {
                    if (taken[color] == selected && table[taken[color]].pawns > 0)
                    {
                        return true;
                    }
                          
                    if( home[color] == selected && IsEnding(color))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //Decyduje, ktore z zaznaczonych pol ma byc wybrane
        public int Select(int color, int selected, int matched)
        {
            if(selected == matched)
            {
                return -1;
            }

            if(matched >= 0)
            {
                if (matched < 24)
                {
                    if (table[matched].color == color && table[taken[color]].pawns == 0)
                    {
                        return matched;
                    }
                }
                else
                {
                    if (matched == taken[color] )
                    {
                        return matched;
                    }
                }
            }
            return selected;
        }

        //Sprawdza w jakie pole kliknal gracz i czy istnieje ruch idpowiadajacy temu zaznaczeniu
        public int TrytoSelectField(Player player, Move move, int x, int y, Classes.ClientMove clientmove)
        { 
            int matched = -1;
            if (y < 300)
            {
                if (x > 580)
                {
                    matched = 27;
                }

                if (matched < 0)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (x > 540 - i * 40)
                        {
                            matched = i;
                            break;
                        }
                    }
                }

                if (x > 290 && matched < 0)
                {
                    matched = 24;
                }

                if (matched < 0)
                {
                    for (int i = 6; i < 12; i++)
                    {
                        if (x > 250 - (i - 6) * 40)
                        {
                            matched = i;
                            break;
                        }
                    }
                }
            }
            else
            {
                if (y > 400)
                { 
                    if (x > 580)
                    {
                        matched = 26;
                    }
                    if (matched < 0)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            if (x > 540 - i * 40)
                            {
                                matched = 23 - i;
                                break;
                            }
                        }
                    }

                    if (x > 290 && matched < 0)
                    {
                        matched = 25;
                    }

                    if (matched < 0)
                    {
                        for (int i = 6; i < 12; i++)
                        {
                            if (x > 250 - (i - 6) * 40)
                            {
                                matched = 23 - i;
                                break;
                            }
                        }
                    }
                }
            }
            
            if(CanSelect(player.color, matched))
            {
                if(matched >= 0 && selected >= 0)
                {
                    if(selected != matched)
                    {
                        int movemade = Move(player, move, selected, matched);
                        if(movemade > 0)
                        {
                            clientmove.from = selected;
                            clientmove.to = matched;
                            clientmove.endturn = movemade - 1;

                            //Koniec gry
                            if (clientmove.endturn == 4)
                            {
                                movemade = 4;
                            }
                            else
                            {
                                //Brak mozliwych ruchow
                                if (clientmove.endturn == 0 && !isAnyPossibleMove(move))
                                {
                                    clientmove.endturn = 1;
                                    movemade = 3;
                                }
                            }
                            selected = -1;
                            PossibleMoves(selected, move, possiblemoves);
                            return movemade;
                        }
                    } 
                }
            }
            else
            {
                matched = -1;
            }
            selected = Select(player.color, selected, matched); 
            PossibleMoves(selected, move, possiblemoves);
            return 0;
        }

        //Sprawdza, czy istnieje jakikolwiek mozliwy ruch
        public bool isAnyPossibleMove(Move move)
        {
            int[] possible = new int[2];

            if (table[taken[move.color]].pawns == 0)
            {
                for (int i = 0; i < 24; i++)
                {
                    if (table[i].pawns > 0 && table[i].color == move.color)
                    {
                        PossibleMoves(i, move, possible);
                        if (possible[0] >= 0 || possible[1] >= 0)
                        {
                            return true;
                        }
                    }
                }
            }
            else
            {
                PossibleMoves(taken[move.color], move, possible);
                if (possible[0] >= 0 || possible[1] >= 0)
                {
                    return true;
                }
            }
            return false;
        }

        //Sumuje pionki znajdujace sie kolo domu
        private int PawnsInHome(int color)
        {
            int sum = 0;
            if (color == 0)
            {
                for (int i = 18; i < 24; i++)
                {
                    if (table[i].color == 0)
                        sum += table[i].pawns;
                }
            }
            else
            {
                if (color == 1)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        if (table[i].color == 1)
                            sum += table[i].pawns;
                    }
                }
            }
            return sum;
        }

        //Sprawdza czy jest jakikolwiek pionek w domu dalej niz ten, ktory chcemy przesunac (jesli jest, nie mozemy przeniesc za pomoca kosci wiekszej niz zaznaczone pole)
        private bool isPawnInHomeFurther(int color, int pos)
        {
            if (color == 0)
            {
                for (int i = 18; i < pos; i++)
                {
                    if (table[i].color == 0 && table[i].pawns > 0)
                        return true;
                }
            }
            else
            {
                if (color == 1)
                {
                    for (int i = pos + 1; i < 6; i++)
                    {
                        if (table[i].color == 1 && table[i].pawns > 0)
                            return true;
                    }
                }
            }
            return false;
        }

        //Zwraca prawde jesli mozna wprowadzac pionki do domu
        private bool IsEnding(int color)
        {
            if (table[home[color]].pawns + PawnsInHome(color) == 15)
            {
                return true;
            }
            return false;
        }
    }
}
