using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;


namespace Backgammon
{
    //Klasa odpowiedzialna za wszelka komunikacje z serwerem 
    public class ServerCommunicator
    {
        public GameForm client;
        private SocketStateObject socket;
        public Queue<string> servermessage;
        public Queue<string> clientmessage;
        public bool nowriting;
        public bool unpacking;
        public int timermessage;

        public ServerCommunicator(GameForm client)
        {
            this.client = client;
            servermessage = new Queue<string>();
            clientmessage = new Queue<string>();
            nowriting = true;
            unpacking = false;
            timermessage = TM_STILL_HERE;
        }

        public class SocketStateObject
        {
            public Socket m_SocketFd = null;
            public StringBuilder m_StringBuilder = new StringBuilder();
            public const int BUF_SIZE = 15;
            public byte[] m_DataBuf = new byte[BUF_SIZE];
            public int bytessended = 0;
            public int bytessrecived = 0;
        }

        //Konczy polaczenie z serwerem
        public void CloseSocket()
        {
            if(socket != null)
            {
                    /* shutdown and close socket */
                    socket.m_SocketFd.Shutdown(SocketShutdown.Both);
                    socket.m_SocketFd.Close();
            }
        }

        //Stale wskazujace jaka wiadomosc ma byc wysylana przez ServerTimerMessage
        private const int TM_STILL_HERE = 0;
        private const int TM_GIVE_OPP = 1;
        private const int TM_GIVE_START_DICES = 2;
        private const int TM_GIVE_OPP_DICES = 3;
        private const int TM_GIVE_OPP_MOVE = 4;
        private const int TM_NEXT_GAME = 5;
        private const int TM_NOT_READY = 6;

        //Wysylana co jakis czas wiadomosc do serwera (wywolywana przez timer)
        public void ServerTimerMessage()
        {
            if (clientmessage.Count == 0 && socket != null && nowriting && !unpacking)
            {
                switch (timermessage)
               {
                   case TM_STILL_HERE:
                       {
                           PreaperMessageCI(30);
                       }break;
                   case TM_GIVE_OPP:
                       {
                           PreaperMessageCI(40);
                       } break;
                   case TM_GIVE_START_DICES:
                       {
                           PreaperMessageCG(11);
                       } break;
                   case TM_GIVE_OPP_DICES:
                       {
                           PreaperMessageCG(21);
                       } break;
                   case TM_GIVE_OPP_MOVE:
                       {
                           PreaperMessageCG(40);
                       } break;
                   case TM_NEXT_GAME:
                       {
                           PreaperMessageCG(61);
                       } break;
                   case TM_NOT_READY:
                       {
                           PreaperMessageCG(80);
                       } break;
                   default: { } break;
                }
            }
        }

        //Uzyskuje socket i wysyla pierwsza wiadomosc (login)
        public void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the socket from the state object */
                Socket socketFd = (Socket)ar.AsyncState;

                /* complete the connection */
                socketFd.EndConnect(ar);

                /* create the SocketStateObject */
                socket = new SocketStateObject();
                socket.m_SocketFd = socketFd;

                this.client.LoginFormEnd();
                client.setButton(client.NewGameButton, true);
                PreaperMessageCI(10, client.clientnick);
            }
            catch (Exception exc)
            {
                this.client.login.setButton(true);
                this.client.login.setLogginErrorLabel(true);
            }
        }

        //Odczytuje wiadomosc od serwera
        public void Recive()
        {
            socket.bytessrecived = 0;
            socket.m_StringBuilder.Clear();
            /* begin receiving the data */
            socket.m_SocketFd.BeginReceive(socket.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(ReceiveCallback), socket);
        }
        
        //Odczytuje przeslana wiadomosc od serwera, jesli jakas wiadomosc czeka w kolejce do wyslania to wysyla ja
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                /* read data */
                int size = socketFd.EndSend(ar);
                socket.bytessrecived += size;
                if (socket.bytessrecived < 15)
                {
                    /* get the rest of the data */
                    socketFd.BeginReceive(state.m_DataBuf, socket.bytessended, SocketStateObject.BUF_SIZE - socket.bytessended, 0, new AsyncCallback(ReceiveCallback), state);
                }
                else
                { 
                    /* all the data has arrived */
                    state.m_StringBuilder.Append(Encoding.ASCII.GetString(state.m_DataBuf, 0, size));
                    servermessage.Enqueue(state.m_StringBuilder.ToString());

                    if(clientmessage.Count == 0)
                    {
                        nowriting = true;
                    }
                    else
                    {
                        socket.bytessended = 0;
                        string mess = clientmessage.Dequeue();
                        socket.m_DataBuf = Encoding.ASCII.GetBytes(mess);

                        /* begin sending the data */
                        socket.m_SocketFd.BeginSend(socket.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(SendCallback), socket);
                    }
                    if (!unpacking)
                    {
                        UnpackMessage();
                    }
                }
            }
            catch (Exception exc)
            {
                client.NewInfoWidnow("Serwer został rozłączony!\n");
                Application.Exit();
            }
        }

        //Wysyla wiadomosc
        private void Send(string mess)
        {
            if (mess.Length == 15)
            {
                if (nowriting)
                {
                    nowriting = false;
                    socket.bytessended = 0;
                    socket.m_DataBuf = Encoding.ASCII.GetBytes(mess);

                    /* begin sending the data */
                    socket.m_SocketFd.BeginSend(socket.m_DataBuf, 0, SocketStateObject.BUF_SIZE, 0, new AsyncCallback(SendCallback), socket);
                }
                else
                {
                    clientmessage.Enqueue(mess);
                }
            }
            else
            {
                client.NewInfoWidnow("Message to short");
            }
        }

        //Sprawdza stan przeslanej wiadomosc do serwera, jesli odebrana cala wiadomosc, to czyta odpowiedz serwera
        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                /* retrieve the SocketStateObject */
                SocketStateObject state = (SocketStateObject)ar.AsyncState;
                Socket socketFd = state.m_SocketFd;

                /* read data */
                socket.bytessended = socketFd.EndReceive(ar);

                if (socket.bytessended < 15)
                {
                    /* send the rest of the data */
                    socketFd.BeginSend(state.m_DataBuf, socket.bytessended, SocketStateObject.BUF_SIZE - socket.bytessended, 0, new AsyncCallback(SendCallback), state);
                }
                else
                {
                    /* all the data sended */
                    Recive();
                }
            }
            catch (Exception exc)
            {
                client.NewInfoWidnow("Serwer został rozłączony!\n");
                Application.Exit();
            }
        }

        //Wypelnia wiadomosc do rozmiaru 15 znakiem \0 i wysyla wiadomosc
        private void PreaperMessage(string mess)
        {
            mess = mess.PadRight(15, '\0');
            Send(mess);
        }

        //Przygotowuje wiadomosc typu informacyjnego - opcjonalne parametry nalezy wypelnic w zaleznosci od potrzeb danego komunikatu
        public void PreaperMessageCI(int code, string clientnick = "")
        {
            bool good = true;
            string mess = "CI" + code.ToString();
            switch (code)
            {
                case 10:
                    {
                        if(clientnick.Length != 0)
                        {
                            mess += client.clientnick;
                        }
                        else
                        {
                            good = false;
                            client.NewInfoWidnow("Client nick cannot be empty"); 
                        }
                    } break;
                case 20:
                    {
                    } break;
                case 30:
                    {
                    } break;
                case 40:
                    {
                        client.setLabelVisible(client.OpponentSearchLabel, true);
                    } break;
                default:
                    {
                        good = false;
                        client.NewInfoWidnow("Unknown client info message"); 
                    } break;
            }
            if (good)
            {
                PreaperMessage(mess);
            }
        }

        //Przygotowuje wiadomosc typu game - opcjonalne parametry nalezy wypelnic w zaleznosci od potrzeb danego komunikatu
        public void PreaperMessageCG(int code, int from = -1, int to = -1, int endofturn = -1, int winner = -1, int decision = -1)
        {
            bool good = true;
            string mess = "CG" + code.ToString();
            switch (code)
            {
                case 10:
                    {
                    } break;
                case 11:
                    {   
                    } break;
                case 20:
                    {
                    } break;
                case 21:
                    {
                    } break;
                case 30:
                    {
                        if(from >= 0 && to >= 0 && endofturn >=0)
                        {
                            mess += from.ToString().PadLeft(2, '0') + to.ToString().PadLeft(2, '0') + endofturn.ToString();
                            if(endofturn > 0)
                            {
                                timermessage = TM_GIVE_OPP_DICES;
                            }
                        }
                        else
                        {
                            good = false;
                            client.NewInfoWidnow("Can't make move: from: " + from.ToString() + ", to:" + to.ToString() + ", endofturn: " + endofturn.ToString());
                        }
                    } break;
                case 40:
                    {
                    } break;
                case 50:
                    {
                        if (winner >=0)
                        {
                            mess += winner.ToString();

                            if(client.game.player.color == winner)
                            {
                                client.NewInfoWidnow("Game end, you win!");
                            }
                            else
                            {
                                client.NewInfoWidnow("Game end, you lose!");
                            }
                        }
                        else
                        {
                            good = false;
                            client.NewInfoWidnow("End of game have to have winner filled");
                        }
                    } break;
                case 60:
                    {
                        if (decision >= 0)
                        {
                            mess += decision.ToString();
                        }
                        else
                        {
                            good = false;
                            client.NewInfoWidnow( "New game have to have decision filled");
                        }
                        //todo
                    } break;
                case 61:
                    {
                    } break;
                case 70:
                    {
                    } break;
                case 80:
                    {
                    } break;
                default:
                    {
                        good = false;
                        client.NewInfoWidnow("Unknown client info message");
                    } break;
            }
            if (good)
            {
                PreaperMessage(mess);
            }
        }

        //Przygotowuje wiadomosc typu error
        public void PreaperMessageCE(int code)
        {
            bool good = true;
            string mess = "CE" + code.ToString();
            switch (code)
            {
                case 10:
                    {
                    } break;
                default:
                    {
                        good = false;
                        client.NewInfoWidnow("Unknown client info message");
                    } break;
            }
            if(good)
                PreaperMessage(mess);
        }

        //Odpakowuje wiadomosc zgodnie z kolejnoscia otrzymania
        public void UnpackMessage()
        {
            unpacking = true;
            string mess = servermessage.Dequeue();
                
            if(mess[0] == 'S')
            {
                if(mess[1] == 'I')
                {
                    UnpackIMessage(mess);
                }

                if (mess[1] == 'G')
                {
                    UnpackGMessage(mess);
                }

                if (mess[1] == 'E')
                {
                    UnpackEMessage(mess);
                }
            }
            else
            {
                client.NewInfoWidnow("Unknown server message");
            }

            if(servermessage.Count == 0)
            {
                unpacking = false;
            }
            else
            {
                UnpackMessage();
            }
        }

        //Interprate Information message
        public void UnpackIMessage(string mess)
        {
            int code = int.Parse(mess.Substring(2, 2));
            switch (code)
            {
                case 10:
                    {
                    } break;
                case 11:
                    {
                        client.NewInfoWidnow("Wrong nick");
                    } break;
                case 20:
                    {
                        client.EndGame();
                    } break;
                case 30:
                    {
                    } break;
                case 40:
                    {
                        timermessage = TM_STILL_HERE;
                        client.opponentnick = mess.Substring(4, 10);
                        client.setLabelVisible(client.OpponentSearchLabel, false);
                        PreaperMessageCG(10);
                    } break;
                case 41:
                    {
                        client.OpponentSearchInc();
                        timermessage = TM_GIVE_OPP;
                    } break;
                default:
                    {
                        client.NewInfoWidnow("Unknown server message");
                    } break;
            }
        }

        //Interprate Game message
        public void UnpackGMessage(string mess)
        {
            int code = int.Parse(mess.Substring(2, 2));
            switch (code)
            {
                case 10:
                    {
                        client.NewGame(int.Parse(mess.Substring(5, 2)), int.Parse(mess.Substring(7, 2)), int.Parse(mess.Substring(4, 1)));
                        client.setButtonVisible(client.RollDicesButton, true);
                    } break;
                case 11:
                    {
                        client.game.SetStartDices(int.Parse(mess.Substring(4, 1)), int.Parse(mess.Substring(5, 1)));
                        if (client.game.GetStartDices()[0] == 0 || client.game.GetStartDices()[1] == 0)
                        {
                            timermessage = TM_GIVE_START_DICES;
                        }
                        else
                        {
                            timermessage = TM_NOT_READY;

                            if (client.game.GetStartDices()[0] > client.game.GetStartDices()[1])
                            {
                                client.NewInfoWidnow("Your turn");
                            }

                            if(client.game.GetStartDices()[0] < client.game.GetStartDices()[1])
                            {
                                client.NewInfoWidnow("Opponent turn");
                            }

                            int gamestart = client.game.GameStart();

                            switch (gamestart)
                            {
                                case 0:
                                    {
                                        timermessage = TM_GIVE_OPP_DICES;
                                    } break;

                                case 1:
                                    {
                                        if (client.game != null)
                                        {
                                            client.setButtonVisible(client.RollDicesButton, true);
                                        }
                                        else
                                        {
                                            client.setButtonVisible(client.RollDicesButton, false);
                                        }
                                    } break;

                                case 2:
                                    {
                                        if (client.game != null)
                                        {
                                            client.NewInfoWidnow("Draw!");
                                            client.setButtonVisible(client.RollDicesButton, true);
                                            timermessage = TM_NOT_READY;
                                        }
                                        else
                                        {
                                            client.setButtonVisible(client.RollDicesButton, false);
                                        }
                                    } break;
                            }
                        }
                    } break;
                case 20:
                    {
                        if(!client.game.SetPlayerDices(int.Parse(mess.Substring(4, 1)), int.Parse(mess.Substring(5, 1))))
                        {
                            client.NewInfoWidnow("No possible moves");
                            client.game.EndTurn();
                        }
                    } break;
                case 21:
                    {
                        if(!client.game.SetOpponentDices(int.Parse(mess.Substring(4, 1)), int.Parse(mess.Substring(5, 1))))
                        {
                            client.NewInfoWidnow("No possible moves");
                            client.game.EndTurn();
                        }

                        if (int.Parse(mess.Substring(4, 1)) == 0 || int.Parse(mess.Substring(5, 1)) == 0)
                        {
                            timermessage = TM_GIVE_OPP_DICES;
                        }
                        else
                        {
                            timermessage = TM_GIVE_OPP_MOVE;
                        }
                    } break;
                case 30:
                    {
                    } break;
                case 40:
                    {
                        int move = client.game.board.Move(client.game.opponent, client.game.opponentmove, int.Parse(mess.Substring(4, 2)), int.Parse(mess.Substring(6, 2)));

                        if (move == 4)
                        {
                            client.GivePointTo(client.game.player.color);
                            PreaperMessageCG(50, winner: client.game.opponent.color);
                        }
                        else
                        {
                            if (move - 1 == int.Parse(mess.Substring(8, 1)))
                            {

                                if (int.Parse(mess.Substring(8, 1)) == 1)
                                {
                                    if (client.game.EndTurn())
                                    {
                                        client.game.SetPlayerDices(0, 0);
                                        client.NewInfoWidnow("Your turn");
                                        client.setButtonVisible(client.RollDicesButton, true);
                                    }
                                    timermessage = TM_NOT_READY;
                                }
                                timermessage = TM_GIVE_OPP_MOVE;
                            }
                            else
                            {
                                client.NewInfoWidnow("Client and server moves not equal");
                                Application.Exit();
                            }
                        }
                        
                    } break;
                case 41:
                    {
                        timermessage = TM_GIVE_OPP_MOVE;
                    } break;
                case 42:
                    {
                        client.game.turn = client.game.player.color;
                    } break;
                case 50:
                    {
                        int decision;
                        timermessage = TM_NEXT_GAME;
                        if (MessageBox.Show(new Form() { TopMost = true }, "Do you want to play aggaint with " + client.opponentnick, "Backgammon", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            decision = 1;    
                        }
                        else
                        {
                            decision = 0;
                            client.ResetGameForm();
                            timermessage = TM_STILL_HERE; 
                        }
                        PreaperMessageCG(60, decision: decision);
                    } break;
                case 60:
                    {
                    } break;
                case 61:
                    {
                        if(int.Parse(mess.Substring(4, 1)) == 0)
                        {
                            timermessage = TM_STILL_HERE;
                            client.ResetGameForm();
                        }
                        else
                        {
                            timermessage = TM_NOT_READY;
                            PreaperMessageCG(10);
                        }
                    } break;
                case 62:
                    {
                        timermessage = TM_NEXT_GAME;
                    } break;
                case 70:
                    {
                        client.Surrender();
                    } break;
                case 80:
                    {
                    } break;

                default:
                    {
                        client.NewInfoWidnow("Unknown server message");
                    } break;
            }
        }

        //Interprate Error message
        public void UnpackEMessage(string mess)
        {
            int code = int.Parse(mess.Substring(2, 2));
            switch(code)
            {
                case 10:
                    {
                        client.NewInfoWidnow("Unknown message");
                    }break;
                case 11:
                    {
                        client.NewInfoWidnow("Unknown message ack");
                    } break;
                case 20:
                    {
                        client.NewInfoWidnow("Yau're not logged in");
                    } break;
                case 30:
                    {
                       client.NewInfoWidnow("You don't have a game");
                       client.ResetGameForm();
                        timermessage = TM_STILL_HERE;
                    } break;
                case 40:
                    {
                        client.NewInfoWidnow("Not your turn");
                    } break;
                case 50:
                    {
                        client.NewInfoWidnow("Game not started - roll dices");
                    } break;
                case 51:
                    {
                        client.NewInfoWidnow("You haven't rool dices");
                    } break;
                case 52:
                    {
                        client.NewInfoWidnow("You have unread opponent moves on server");
                    } break;
                case 53:
                    {
                        client.NewInfoWidnow("Game Ended");
                    } break;
                case 54:
                    {
                        client.NewInfoWidnow("Game hasn't end yet");
                    } break;
                case 60:
                    {
                        client.NewInfoWidnow("Opponent left game");
                        client.ResetGameForm();
                        timermessage = TM_STILL_HERE;
                        client.setButton(client.NewGameButton, true);
                        client.setButtonVisible(client.RollDicesButton, false);
                    } break;
                default:
                    {
                        client.NewInfoWidnow("Unknown server message");
                    } break;
            }
        }
    }
}