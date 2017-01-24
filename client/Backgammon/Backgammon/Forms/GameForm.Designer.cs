namespace Backgammon
{
    partial class GameForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GameForm));
            this.Board = new System.Windows.Forms.PictureBox();
            this.BoardTimer = new System.Windows.Forms.Timer(this.components);
            this.PlayerWhiteNick = new System.Windows.Forms.Label();
            this.PlayerWhiteScore = new System.Windows.Forms.Label();
            this.PlayerRedNick = new System.Windows.Forms.Label();
            this.NewGameButton = new System.Windows.Forms.Button();
            this.ExitButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.PlayerRedScore = new System.Windows.Forms.Label();
            this.RollDicesButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.Serwertimer = new System.Windows.Forms.Timer(this.components);
            this.WaitOpponentDicesLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Board)).BeginInit();
            this.SuspendLayout();
            // 
            // Board
            // 
            resources.ApplyResources(this.Board, "Board");
            this.Board.Name = "Board";
            this.Board.TabStop = false;
            this.Board.Click += new System.EventHandler(this.Board_Click);
            this.Board.Paint += new System.Windows.Forms.PaintEventHandler(this.Board_Paint);
            // 
            // BoardTimer
            // 
            this.BoardTimer.Enabled = true;
            this.BoardTimer.Interval = 10;
            this.BoardTimer.Tick += new System.EventHandler(this.BoardTimer_Tick);
            // 
            // PlayerWhiteNick
            // 
            resources.ApplyResources(this.PlayerWhiteNick, "PlayerWhiteNick");
            this.PlayerWhiteNick.Name = "PlayerWhiteNick";
            // 
            // PlayerWhiteScore
            // 
            resources.ApplyResources(this.PlayerWhiteScore, "PlayerWhiteScore");
            this.PlayerWhiteScore.Name = "PlayerWhiteScore";
            // 
            // PlayerRedNick
            // 
            resources.ApplyResources(this.PlayerRedNick, "PlayerRedNick");
            this.PlayerRedNick.Name = "PlayerRedNick";
            // 
            // NewGameButton
            // 
            resources.ApplyResources(this.NewGameButton, "NewGameButton");
            this.NewGameButton.Name = "NewGameButton";
            this.NewGameButton.UseVisualStyleBackColor = true;
            this.NewGameButton.Click += new System.EventHandler(this.NewGameButton_Click);
            // 
            // ExitButton
            // 
            resources.ApplyResources(this.ExitButton, "ExitButton");
            this.ExitButton.Name = "ExitButton";
            this.ExitButton.UseVisualStyleBackColor = true;
            this.ExitButton.Click += new System.EventHandler(this.ExitButton_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // PlayerRedScore
            // 
            resources.ApplyResources(this.PlayerRedScore, "PlayerRedScore");
            this.PlayerRedScore.Name = "PlayerRedScore";
            // 
            // RollDicesButton
            // 
            resources.ApplyResources(this.RollDicesButton, "RollDicesButton");
            this.RollDicesButton.Name = "RollDicesButton";
            this.RollDicesButton.UseVisualStyleBackColor = true;
            this.RollDicesButton.Click += new System.EventHandler(this.RollDicesButton_Click);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // Serwertimer
            // 
            this.Serwertimer.Enabled = true;
            this.Serwertimer.Interval = 2000;
            this.Serwertimer.Tick += new System.EventHandler(this.Serwertimer_Tick);
            // 
            // WaitOpponentDicesLabel
            // 
            resources.ApplyResources(this.WaitOpponentDicesLabel, "WaitOpponentDicesLabel");
            this.WaitOpponentDicesLabel.Name = "WaitOpponentDicesLabel";
            // 
            // GameForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ControlBox = false;
            this.Controls.Add(this.WaitOpponentDicesLabel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.RollDicesButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.PlayerRedScore);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ExitButton);
            this.Controls.Add(this.NewGameButton);
            this.Controls.Add(this.PlayerRedNick);
            this.Controls.Add(this.PlayerWhiteScore);
            this.Controls.Add(this.PlayerWhiteNick);
            this.Controls.Add(this.Board);
            this.Name = "GameForm";
            this.ShowIcon = false;
            this.Activated += new System.EventHandler(this.Game_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.GameForm_FormClosing);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.GameForm_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.Board)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox Board;
        private System.Windows.Forms.Timer BoardTimer;
        private System.Windows.Forms.Label PlayerWhiteNick;
        private System.Windows.Forms.Label PlayerWhiteScore;
        private System.Windows.Forms.Label PlayerRedNick;
        public System.Windows.Forms.Button NewGameButton;
        private System.Windows.Forms.Button ExitButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label PlayerRedScore;
        public System.Windows.Forms.Button RollDicesButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Timer Serwertimer;
        private System.Windows.Forms.Label WaitOpponentDicesLabel;
    }
}