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
    public partial class InfoForm : Form
    {
        public InfoForm(string text)
        {
            InitializeComponent();
            this.label1.Text = text;
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }
    }
}
