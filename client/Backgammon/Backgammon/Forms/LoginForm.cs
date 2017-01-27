using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Backgammon
{
    //Klasa okna logowania
    public partial class LoginForm : Form
    {
        GameForm game;
        delegate void setButtonCallback(bool status);
        delegate void setLogginErrorLabelCallback(bool visible);

        public LoginForm(GameForm game)
        {
            InitializeComponent();
            this.game = game;
        }

        private void NicktextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar)  && !char.IsLetterOrDigit(e.KeyChar) && !(e.KeyChar == '_'))
            {
                e.Handled = true;
            }
        }

        public void setButton(bool status)
        {
            if (this.LoginButton.InvokeRequired)
            {
                setButtonCallback buttonCallback = new setButtonCallback(setButton);
                this.Invoke(buttonCallback, status);
            }
            else
            {
                this.LoginButton.Enabled = status;
            }
        }

        public void setLogginErrorLabel(bool visible)
        {
            if (this.LoginErrorLabel.InvokeRequired)
            {
                setLogginErrorLabelCallback labelCallback = new setLogginErrorLabelCallback(setLogginErrorLabel);
                this.Invoke(labelCallback, visible);
            }
            else
            {  
                LoginErrorLabel.Visible = visible;
            }
          
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            setButton(false);

            this.NickErrorLabel.Visible = false;
            this.IPErrorLabel.Visible = false;
            this.LoginErrorLabel.Visible = false;

            string nick = this.NicktextBox.Text;
            bool error = false;
            if(nick.Length == 0)
            {
                error = true;
                this.NickErrorLabel.Text = "Nick can't be empty";
                this.NickErrorLabel.Visible = true;
            }

            if (nick[0] == '_')
            {
                error = true;
                this.NickErrorLabel.Text = "Nick cannot start with '_'";
                this.NickErrorLabel.Visible = true;
            }

            foreach (char i in nick)
                if (!char.IsLetterOrDigit(i) && !(i == '_'))
                {
                    error = true;
                    this.NickErrorLabel.Text = "Nick can consist only of letters, diggits or '_'";
                    this.NickErrorLabel.Visible = true;
                    break;
                }

            string IP = this.ServerIPtextBox.Text;
            
            try
            {
                //try to connect to server
                if (!error)
                {
                    game.clientnick = nick;

                    IPAddress address = IPAddress.Parse(IP);
                    Socket socketFd = null;
                    IPEndPoint endPoint = null;

                    /* create a socket */
                    socketFd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    /* remote endpoint for the socket */
                    endPoint = new IPEndPoint(address, Int32.Parse("3000"));

                    /* connect to the server */
                    socketFd.BeginConnect(endPoint, new AsyncCallback(game.servercommunicator.ConnectCallback), socketFd);
                }
            }
            catch (Exception err)
            {
                error = true;
                this.IPErrorLabel.Visible = true;
            }   

            if(error)
            {
                setButton(true);
            }
        }

        private void ExitButton_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Do you really want to exit?", "Backgammon", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, (MessageBoxOptions)0x40000) == DialogResult.Yes)
            {
                game.EndGame();
            }
        }
    }
}
