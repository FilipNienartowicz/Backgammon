using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//Glowna klasa - glowne okno aplikacji
namespace Backgammon
{
    public partial class GameForm : Form
    {
        public LoginForm login;
        private GameGraphics graphics;

        public Classes.Game game;
        public string opponentnick;
        public string clientnick;
        private int opponentsearch;
        private bool clickable;

        public ServerCommunicator servercommunicator;

        public GameForm()
        {
            InitializeComponent();
            graphics = new GameGraphics();
            this.login = new LoginForm(this);
            login.Visible = true;
            
            servercommunicator = new ServerCommunicator(this);
            opponentnick = "guest";
            opponentsearch = 0;
            setButton(this.NewGameButton, false);
            clickable = true;
        }

        //Przerzuca inne okna, gdy aktywne razem z oknem gry
        private void Game_Activated(object sender, EventArgs e)
        {
            if (login != null && login.Visible == true)
            {
                login.Activate();
            }
        }

        //Ustawia LabelVisible
        delegate void setLabelVisibleCallback(Label label, bool visible);
        public void setLabelVisible(Label label, bool visible)
        {
            if (label.InvokeRequired)
            {
                setLabelVisibleCallback labelCallback = new setLabelVisibleCallback(setLabelVisible);
                this.Invoke(labelCallback, label, visible);
            }
            else
            {
                label.Visible = visible;
            }
        }

        //Ustawia LabelText
        delegate void setLabelTextCallback(Label label, string text);
        public void setLabelText(Label label, string text)
        {
            if (label.InvokeRequired)
            {
                setLabelTextCallback labelCallback = new setLabelTextCallback(setLabelText);
                this.Invoke(labelCallback, label, text);
            }
            else
            {
                label.Text = text;
            }
        }

        //Tworzy okno informacyjne z tekstem podanym jako parametr
        delegate void NewInfoWidnowCallback(string text);
        public void NewInfoWidnow(string text)
        {
            if (this.InvokeRequired)
            {
                NewInfoWidnowCallback windowCallback = new NewInfoWidnowCallback(NewInfoWidnow);
                this.Invoke(windowCallback, text);
            }
            else
            {
                clickable = false;
                MessageBox.Show(new Form(){TopMost = true},text, "Backgammon",MessageBoxButtons.OK);
                clickable = true;
            }
        }

        //Tworzy nowa gre
        public void NewGame(int playerscore, int opponentscore, int color)
        {
            string col = "red";
            if (color == 0)
            {
                col = "white";
            }
            NewInfoWidnow("Opponent found. Your color: " + col);
            setButton(NewGameButton, true);
            game = new Classes.Game(clientnick, playerscore, opponentnick, opponentscore, color);
            if(color == 0)
            {
                setLabelText(PlayerRedNick, opponentnick);
                setLabelText(PlayerRedScore, opponentscore.ToString());
                setLabelText(PlayerWhiteNick, clientnick);
                setLabelText(PlayerWhiteScore, playerscore.ToString());
            }
            else
            {
                setLabelText(PlayerRedNick, clientnick);
                setLabelText(PlayerRedScore, playerscore.ToString());
                setLabelText(PlayerWhiteNick, opponentnick);
                setLabelText(PlayerWhiteScore, opponentscore.ToString());
            } 
        }

        public void ResetGameForm()
        {
            game = null;
            setLabelText(PlayerRedNick, "guest");
            setLabelText(PlayerWhiteNick, "guest");
            setLabelText(PlayerRedScore, "0");
            setLabelText(PlayerWhiteScore, "0");
            opponentnick = "guest";
            setButton(NewGameButton, true);
            setButtonVisible(RollDicesButton, false);
            setLabelVisible(OpponentSearchLabel,false);
        }

        //Konczy program
        public void EndGame()
        {
            servercommunicator.CloseSocket();
            this.Dispose();
        }

        public void Surrender()
        {
            ResetGameForm();
        }
        
        //Exit klikniete
        private void ExitButton_Click(object sender, EventArgs e)
        {
            if (clickable)
            {
                clickable = false;
                if (MessageBox.Show(new Form(){TopMost = true},"Do you really want to exit?", "Backgammon",MessageBoxButtons.YesNo,MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    this.EndGame();
                }
                clickable = true;
            }
        }

        //zamyka socket, gdy zamykany jest program
        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            servercommunicator.CloseSocket();
        }

        //Ustawia Button
        delegate void setButtonCallback(Button button, bool status);
        public void setButton(Button button,  bool status)
        {
            if (button.InvokeRequired)
            {
                setButtonCallback buttonCallback = new setButtonCallback(setButton);
                this.Invoke(buttonCallback, button, status);
            }
            else
            {
                button.Enabled = status;
            }
        }

        //Ustawia Buttonvisible
        delegate void setButtonVisibleCallback(Button button, bool status);
        public void setButtonVisible(Button button, bool status)
        {
            if (button.InvokeRequired)
            {
                setButtonVisibleCallback buttonCallback = new setButtonVisibleCallback(setButtonVisible);
                this.Invoke(buttonCallback, button, status);
            }
            else
            {
                button.Visible = status;
            }
        }

        //New game klikniete
        private void NewGameButton_Click(object sender, EventArgs e)
        {
            if (clickable)
            {
                setButton(this.NewGameButton, false);
                if (game == null)
                {
                    servercommunicator.PreaperMessageCI(40);
                }
                else
                {
                    clickable = false;
                    if (MessageBox.Show(new Form() { TopMost = true }, "Do you really want to quit the current game?", "Backgammon", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                    {
                        ResetGameForm();
                        setLabelVisible(OpponentSearchLabel, true);
                        servercommunicator.PreaperMessageCG(70);
                        servercommunicator.PreaperMessageCI(40);
                    }
                    else
                    {
                        setButton(this.NewGameButton, true);
                    }
                    clickable = true; 
                }
            }
        }

        //Roll Dices klikniete
        private void RollDicesButton_Click(object sender, EventArgs e)
        {
            if (clickable && game!=null)
            {
                setButtonVisible(this.RollDicesButton, false);
                if (game.turn == 2)
                {
                    servercommunicator.PreaperMessageCG(11);
                }
                else
                {
                    servercommunicator.PreaperMessageCG(20);
                }
            }
            else
            {
                setButtonVisible(this.RollDicesButton, false);
            }
        }

        //Zamyka login
        delegate void LoginFormEndCallback();
        public void LoginFormEnd()
        {
            if (this.login.InvokeRequired)
            {
                LoginFormEndCallback loginback = new LoginFormEndCallback(LoginFormEnd);
                this.Invoke(loginback);
            }
            else
            {
                if (this.login != null)
                {
                    this.login.Dispose();
                } 
            }
        }

        //Klikniecie planszy
        private void Board_Click(object sender, EventArgs e)
        {
            if (clickable)
            {
                MouseEventArgs click = (MouseEventArgs)e;
                if (game != null)
                {
                    Classes.ClientMove clientmove = new Classes.ClientMove();
                    int move = game.SelectField(click.X, click.Y, clientmove);
                    if (move > 0)
                    {
                        if(move == 3)
                        {
                            NewInfoWidnow("No possible moves");
                        }

                        servercommunicator.PreaperMessageCG(30, clientmove.from, clientmove.to, clientmove.endturn);

                        if (move == 4)
                        {
                            GivePointTo(game.player.color);
                            servercommunicator.PreaperMessageCG(50, winner: game.player.color);
                        }
                    }
                }
            }
        }

        //Zwieksza opponentsearch - gdy n razy dostanie odpowiedz negatywna 
        public void OpponentSearchInc()
        {
            //todo
            opponentsearch++;
            if (opponentsearch > 60)
            {
                servercommunicator.timermessage = 0;
                opponentsearch = 0;
                NewInfoWidnow("No opponent available");
            }
        }

        //Jesli uplynal dluzszy czas, to wymusza wyslanie wiadomosci do serwera
        private void Serwertimer_Tick(object sender, EventArgs e)
        {
            servercommunicator.ServerTimerMessage();
        } 

        //Timer, ktory wywoluje odsiwezenie ekranu
        private void BoardTimer_Tick(object sender, EventArgs e)
        {
            Board.Image = this.graphics.CreateGameBoard(game);
            this.Board.Refresh();
        }

        public void GivePointTo(int color)
        {
            if(color == 0)
            {
                setLabelText(PlayerWhiteScore, (int.Parse(PlayerWhiteScore.Text) + 1).ToString());
            }
            else
            {
                setLabelText(PlayerRedScore, (int.Parse(PlayerRedScore.Text) + 1).ToString());
            }
        }
    }
}
