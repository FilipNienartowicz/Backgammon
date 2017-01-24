namespace Backgammon
{
    partial class LoginForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LoginButton = new System.Windows.Forms.Button();
            this.NickLabel = new System.Windows.Forms.Label();
            this.NicktextBox = new System.Windows.Forms.TextBox();
            this.ServerIPtextBox = new System.Windows.Forms.TextBox();
            this.ServerIPLabel = new System.Windows.Forms.Label();
            this.NickErrorLabel = new System.Windows.Forms.Label();
            this.IPErrorLabel = new System.Windows.Forms.Label();
            this.LoginErrorLabel = new System.Windows.Forms.Label();
            this.ExitButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LoginButton
            // 
            this.LoginButton.Location = new System.Drawing.Point(36, 215);
            this.LoginButton.Name = "LoginButton";
            this.LoginButton.Size = new System.Drawing.Size(137, 78);
            this.LoginButton.TabIndex = 0;
            this.LoginButton.Text = "Login";
            this.LoginButton.UseVisualStyleBackColor = true;
            this.LoginButton.Click += new System.EventHandler(this.LoginButton_Click);
            // 
            // NickLabel
            // 
            this.NickLabel.AutoSize = true;
            this.NickLabel.Location = new System.Drawing.Point(64, 88);
            this.NickLabel.Name = "NickLabel";
            this.NickLabel.Size = new System.Drawing.Size(35, 17);
            this.NickLabel.TabIndex = 1;
            this.NickLabel.Text = "Nick";
            // 
            // NicktextBox
            // 
            this.NicktextBox.HideSelection = false;
            this.NicktextBox.Location = new System.Drawing.Point(158, 88);
            this.NicktextBox.MaxLength = 10;
            this.NicktextBox.Name = "NicktextBox";
            this.NicktextBox.Size = new System.Drawing.Size(146, 22);
            this.NicktextBox.TabIndex = 2;
            this.NicktextBox.Text = "guest";
            this.NicktextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.NicktextBox_KeyPress);
            // 
            // ServerIPtextBox
            // 
            this.ServerIPtextBox.Location = new System.Drawing.Point(161, 150);
            this.ServerIPtextBox.MaxLength = 15;
            this.ServerIPtextBox.Name = "ServerIPtextBox";
            this.ServerIPtextBox.Size = new System.Drawing.Size(143, 22);
            this.ServerIPtextBox.TabIndex = 4;
            this.ServerIPtextBox.Text = "192.168.56.101";
            // 
            // ServerIPLabel
            // 
            this.ServerIPLabel.AutoSize = true;
            this.ServerIPLabel.Location = new System.Drawing.Point(67, 150);
            this.ServerIPLabel.Name = "ServerIPLabel";
            this.ServerIPLabel.Size = new System.Drawing.Size(66, 17);
            this.ServerIPLabel.TabIndex = 3;
            this.ServerIPLabel.Text = "Server IP";
            // 
            // NickErrorLabel
            // 
            this.NickErrorLabel.AutoSize = true;
            this.NickErrorLabel.ForeColor = System.Drawing.Color.DarkRed;
            this.NickErrorLabel.Location = new System.Drawing.Point(64, 115);
            this.NickErrorLabel.Name = "NickErrorLabel";
            this.NickErrorLabel.Size = new System.Drawing.Size(46, 17);
            this.NickErrorLabel.TabIndex = 5;
            this.NickErrorLabel.Text = "label1";
            this.NickErrorLabel.Visible = false;
            // 
            // IPErrorLabel
            // 
            this.IPErrorLabel.AutoSize = true;
            this.IPErrorLabel.ForeColor = System.Drawing.Color.DarkRed;
            this.IPErrorLabel.Location = new System.Drawing.Point(84, 186);
            this.IPErrorLabel.Name = "IPErrorLabel";
            this.IPErrorLabel.Size = new System.Drawing.Size(208, 17);
            this.IPErrorLabel.TabIndex = 6;
            this.IPErrorLabel.Text = "IP not correct. e.g. IP: 10.0.2.15";
            this.IPErrorLabel.Visible = false;
            // 
            // LoginErrorLabel
            // 
            this.LoginErrorLabel.AutoSize = true;
            this.LoginErrorLabel.ForeColor = System.Drawing.Color.DarkRed;
            this.LoginErrorLabel.Location = new System.Drawing.Point(98, 313);
            this.LoginErrorLabel.Name = "LoginErrorLabel";
            this.LoginErrorLabel.Size = new System.Drawing.Size(154, 17);
            this.LoginErrorLabel.TabIndex = 7;
            this.LoginErrorLabel.Text = "Can\'t connect to server";
            this.LoginErrorLabel.Visible = false;
            // 
            // ExitButton
            // 
            this.ExitButton.Location = new System.Drawing.Point(198, 215);
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.Size = new System.Drawing.Size(137, 78);
            this.ExitButton.TabIndex = 8;
            this.ExitButton.Text = "Exit";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(370, 359);
            this.ControlBox = false;
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.LoginErrorLabel);
            this.Controls.Add(this.IPErrorLabel);
            this.Controls.Add(this.NickErrorLabel);
            this.Controls.Add(this.ServerIPtextBox);
            this.Controls.Add(this.ServerIPLabel);
            this.Controls.Add(this.NicktextBox);
            this.Controls.Add(this.NickLabel);
            this.Controls.Add(this.LoginButton);
            this.Name = "LoginForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Backgammon";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LoginButton;
        private System.Windows.Forms.Label NickLabel;
        private System.Windows.Forms.TextBox NicktextBox;
        private System.Windows.Forms.TextBox ServerIPtextBox;
        private System.Windows.Forms.Label ServerIPLabel;
        private System.Windows.Forms.Label NickErrorLabel;
        private System.Windows.Forms.Label IPErrorLabel;
        private System.Windows.Forms.Label LoginErrorLabel;
        private System.Windows.Forms.Button ExitButton;
    }
}

