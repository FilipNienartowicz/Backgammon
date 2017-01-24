using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Backgammon
{
    public partial class GameForm : Form
    {
        public LoginForm login;
        private InfoForm info;
        private DecisionForm decision;
        private GameGraphics graphics;

        public Classes.Game game;
        public string opponentnick;
        public string clientnick;
        private int opponentsearch;

        public ServerCommunicator servercommunicator;

        public GameForm()
        {
            InitializeComponent();
            graphics = new GameGraphics();
            this.login = new LoginForm(this);
            login.Visible = true;
            
            this.info = new InfoForm("asdf");
            this.info.Visible = false;

            this.decision = new DecisionForm();
            this.decision.Visible = false;
            
            servercommunicator = new ServerCommunicator(this);
            opponentnick = "guest";
            opponentsearch = 0;
            setButton(this.NewGameButton, false);

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

        //Tworzy nowa gre
        public void NewGame(int playerscore, int opponentscore, int color)
        {
            string col = "red";
            if (color == 0)
            {
                col = "white";
            }
            MessageBox.Show("Opponent found. Your color: " + col);
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

        //Konczy program
        public void EndGame()
        {
            servercommunicator.CloseSocket();
            this.Dispose();
        }

        public void Surrender()
        {
            game = null;
        }
        
        //Exit klikniete
        private void ExitButton_Click(object sender, EventArgs e)
        {
            this.EndGame();
        }

        //zamyka socket, gdy zamykany jest program
        private void GameForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            servercommunicator.CloseSocket();
        }

        //Zwieksza opponentsearch - gdy n razy dostanie odpowiedz negatywna 
        public void OpponentSearchInc()
        {
            //todo
            opponentsearch++;
            if(opponentsearch > 30)
            {
                MessageBox.Show("No opponent available");
            }
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
            setButton(this.NewGameButton, false);
            if(game == null)
            {
                servercommunicator.PreaperMessageCI(40);
            }
            else
            {
                game = null;
                setButtonVisible(this.RollDicesButton, false);
                setLabelVisible(OpponentSearchLabel, true);
                servercommunicator.PreaperMessageCG(70);
                servercommunicator.PreaperMessageCI(40);
            }
        }

        //Roll Dices klikniete
        private void RollDicesButton_Click(object sender, EventArgs e)
        {
            setButtonVisible(this.RollDicesButton, false);
            if(game.turn == 2)
            {
                servercommunicator.PreaperMessageCG(11);
            }
            else
            {
                servercommunicator.PreaperMessageCG(20);
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

        //Jesli uplynal dluzszy czas, to wymusza wyslanie wiadomosci do serwera
        private void Serwertimer_Tick(object sender, EventArgs e)
        {
            this.Update();
        } 
        
        //Wyslanie wymuszonej wiadomosci
        private void GameForm_Paint(object sender, PaintEventArgs e)
        {
            servercommunicator.ServerTimerMessage();
        }

        //Timer, ktory wywoluje odsiwezenie ekranu
        private void BoardTimer_Tick(object sender, EventArgs e)
        {
            this.Board.Update();
        }

        //Wywoluje rysowanie na planszy, gdy jest ona odswiezana
        private void Board_Paint(object sender, PaintEventArgs e)
        {
            Board.Image = this.graphics.CreateGameBoard(game);
        }

        //Klikniecie planszy
        private void Board_Click(object sender, EventArgs e)
        {
            MouseEventArgs click = (MouseEventArgs)e;
            if (game != null)
            {
                Classes.ClientMove clientmove = new Classes.ClientMove();
                int move = game.SelectField(click.X, click.Y, clientmove);
                if(move > 0)
                {
                    servercommunicator.PreaperMessageCG(30, clientmove.from, clientmove.to, clientmove.endturn);
                }
            }
        }
    }
}
