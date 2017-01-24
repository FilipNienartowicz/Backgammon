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
            timermessage = 0;
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

        //Wysylana co jakis czas wiadomosc do serwera (wywolywana przez timer)
        public void ServerTimerMessage()
        {
            if (clientmessage.Count == 0 && socket != null && nowriting)
            {
                switch (timermessage)
               {
                   case 0:
                       {
                           // PreaperMessageCI(30);
                       }break;
                   case 1:
                       {
                           PreaperMessageCI(40);
                       } break;
                   case 2:
                       {
                           PreaperMessageCG(11);
                       } break;
                   case 3:
                       {
                           PreaperMessageCG(21);
                       } break;
                   case 4:
                       {
                           PreaperMessageCG(40);
                       } break;
                   case 5:
                       {
                           PreaperMessageCG(61);
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
                MessageBox.Show("Serwer został rozłączony!\n");
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
                string error = "Message to short";
                MessageBox.Show(error);
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
                MessageBox.Show("Serwer został rozłączony!\n");
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
            string error = ""; 
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
                            error = "Client nick cannot be empty"; 
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
                        error = "Unknown client info message"; 
                    } break;
            }
            if (good)
            {
                PreaperMessage(mess);
            }
            else
            {
                MessageBox.Show(error);
            }
        }

        //Przygotowuje wiadomosc typu game - opcjonalne parametry nalezy wypelnic w zaleznosci od potrzeb danego komunikatu
        public void PreaperMessageCG(int code, int from = -1, int to = -1, int endofturn = -1, int winner = -1, int decision = -1)
        {
            bool good = true;
            string mess = "CG" + code.ToString();
            string error = "";
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
                                timermessage = 3;
                            }
                        }
                        else
                        {
                            good = false;
                            error = "Can't make move: from: " + from.ToString() + ", to:" + to.ToString() + ", endofturn: " + endofturn.ToString();
                        }
                    } break;
                case 40:
                    {
                    } break;
                case 50:
                    {
                        if (winner >= 0)
                        {
                            mess += winner.ToString();
                        }
                        else
                        {
                            good = false;
                            error = "End of game have to have winner filled";
                        }
                    } break;
                case 60:
                    {
                        if (winner >= 0)
                        {
                            mess += decision.ToString();
                        }
                        else
                        {
                            good = false;
                            error = "New game have to have decision filled";
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
                        error = "Unknown client info message";
                    } break;
            }
            if (good)
            {
                PreaperMessage(mess);
            }
            else 
            {
                MessageBox.Show(error);
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
                        string error = "Unknown client info message";
                        MessageBox.Show(error);
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
                MessageBox.Show("Unknown server message");
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
            string error = "";
            switch (code)
            {
                case 10:
                    {
                    } break;
                case 11:
                    {
                        error = "Wrong nick";
                        MessageBox.Show(error);
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
                        timermessage = 0;
                        client.opponentnick = mess.Substring(4, 10);
                        client.setLabelVisible(client.OpponentSearchLabel, false);
                        PreaperMessageCG(10);
                    } break;
                case 41:
                    {
                        client.OpponentSearchInc();
                        timermessage = 1;
                    } break;
                default:
                    {
                        error = "Unknown server message";
                        MessageBox.Show(error);
                    } break;
            }
        }

        //Interprate Game message
        public void UnpackGMessage(string mess)
        {
            int code = int.Parse(mess.Substring(2, 2));
            string error = "";
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
                        if (int.Parse(mess.Substring(4, 1)) == 0 || int.Parse(mess.Substring(5, 1)) == 0)
                        {
                            timermessage = 2;
                        }
                        else
                        {
                            timermessage = 0;
                            if(int.Parse(mess.Substring(4, 1)) != int.Parse(mess.Substring(5, 1)))
                            {
                                if (int.Parse(mess.Substring(4, 1)) > int.Parse(mess.Substring(5, 1)))
                                {
                                    MessageBox.Show("Your turn");
                                }
                                else
                                {
                                    MessageBox.Show("Opponent turn");
                                }
                                
                                int gamestart = client.game.GameStart();
                                
                                if (gamestart == 0)
                                {
                                    timermessage = 3;
                                    
                                }

                                if (gamestart == 1)
                                {
                                    client.setButtonVisible(client.RollDicesButton, true);
                                    
                                }

                                if (gamestart == 2)
                                {
                                    client.setButtonVisible(client.RollDicesButton, true);
                                    timermessage = 0;
                                }
                            }
                            else
                            {
                                client.setButtonVisible(client.RollDicesButton, true);
                                timermessage = 0;
                            }
                        }
                    } break;
                case 20:
                    {
                        client.game.SetPlayerDices(int.Parse(mess.Substring(4, 1)), int.Parse(mess.Substring(5, 1)));
                    } break;
                case 21:
                    {
                        client.game.SetOpponentDices(int.Parse(mess.Substring(4, 1)), int.Parse(mess.Substring(5, 1)));
                        if (int.Parse(mess.Substring(4, 1)) == 0 || int.Parse(mess.Substring(5, 1)) == 0)
                        {
                            timermessage = 3;
                        }
                        else
                        {
                            timermessage = 4;
                        }
                    } break;
                case 30:
                    {
                    } break;
                case 40:
                    {
                        if (client.game.board.Move(client.game.opponent, client.game.opponentmove, int.Parse(mess.Substring(4, 2)), int.Parse(mess.Substring(6, 2))) - 1 == int.Parse(mess.Substring(8, 1)))
                        {
                            if(int.Parse(mess.Substring(8, 1)) == 1)
                            {
                               if(client.game.EndTurn())
                               {
                                    client.setButtonVisible(client.RollDicesButton, true);
                                    MessageBox.Show("Your turn");
                               }         
                               timermessage = 0;
                            }
                            timermessage = 4;
                        }
                        else
                        {
                            error = "Client and server moves not equal";
                            MessageBox.Show(error);
                            Application.Exit();
                        }

                    } break;
                case 41:
                    {
                        timermessage = 4;
                    } break;
                case 42:
                    {
                        client.game.turn = client.game.player.color;
                    } break;
                case 50:
                    {
                    } break;
                case 60:
                    {
                    } break;
                case 61:
                    {
                        timermessage = 0;
                        //todo
                    } break;
                case 62:
                    {
                        timermessage = 5;
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
                        error = "Unknown server message";
                        MessageBox.Show(error);
                    } break;
            }
        }

        //Interprate Error message
        public void UnpackEMessage(string mess)
        {
            int code = int.Parse(mess.Substring(2, 2));
            string error = "";
            switch(code)
            {
                case 10:
                    {
                        error = "Unknown message";
                    }break;
                case 11:
                    {
                        error = "Unknown message ack";
                    } break;
                case 20:
                    {
                        error = "Yau're not logged in";
                    } break;
                case 30:
                    {
                        error = "You don't have a game";
                        client.game = null;
                        timermessage = 0;
                    } break;
                case 40:
                    {
                        error = "Not your turn";
                    } break;
                case 50:
                    {
                        error = "Game not started - roll dices";
                    } break;
                case 51:
                    {
                        error = "You haven't rool dices";
                    } break;
                case 52:
                    {
                        error = "You have unread opponent moves on server";
                    } break;
                case 53:
                    {
                        error = "Game Ended";
                    } break;
                case 54:
                    {
                        error = "Game hasn't end yet";
                    } break;
                case 60:
                    {
                        error = "Opponent left game";
                        client.game = null;
                        client.setButton(client.NewGameButton, true);
                    } break;
                default:
                    {
                        error = "Unknown server message";
                    } break;
            }
            if(code!= 11)
                MessageBox.Show(error);
        }
    }
}