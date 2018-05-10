using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerChoosingDemo
{
    public partial class Form2 : Form
    {
        public string IPScheda;
        public Form2(List<String> list)
        {
            InitializeComponent();
            button1.Enabled = false;
            comboBox1.DataSource = list;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IPScheda = comboBox1.SelectedItem.ToString();
            this.Close();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            button1.Enabled = true;
        }
    }
}
