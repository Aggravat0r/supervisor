using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OC
{
    public partial class Identificate : Form
    {
        public static Form1 y;
        public Identificate()
        {
            InitializeComponent();
        }


        public Identificate(Form1 form)
        {
            InitializeComponent();
            y = form;
        }
        private void label1_Click(object sender, EventArgs e)
        {
            
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {            
            //y.Sys1.Search_User(this);
        }
    }
}
