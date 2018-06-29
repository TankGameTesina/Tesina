using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace provaXNAGame
{
    public partial class FormSelectTank : Form
    {
        public Color ReturnValue1 { get; set; }
        public string ReturnValue2 { get; set; }

        bool usernameSetted;
        bool colorSetted;

        public FormSelectTank()
        {
            InitializeComponent();
            ReturnValue1 = new Color();
            usernameSetted = false;
            colorSetted = false;

            label1.Show();
            tbUsername.Show();
            btnColor.Hide();
            btnStart.Hide();
            lbUsername.Hide();
            label2.Hide();
            label3.Hide();
            pictureBox1.Hide();
            btnContinue.Show();
            btnBack.Text = "<-Esci";
            btnBack.Show();
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            ColorDialog clrDialog = new ColorDialog();

            DialogResult result = clrDialog.ShowDialog();
            // See if user pressed ok.
            if (result == DialogResult.OK)
            {
                // Set form background to the selected color.
                btnColor.BackColor = clrDialog.Color;
                this.ReturnValue1 = clrDialog.Color;
            }

            if (tbUsername.Text != "" && ReturnValue1 != new Color())
                btnStart.Enabled = true;
            else
                btnStart.Enabled = false;
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (tbUsername.Text != "" && ReturnValue1 != new Color())
            {
                this.ReturnValue2 = tbUsername.Text; //example
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
                btnStart.Enabled = false;
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            if (!usernameSetted)
            {
                string username = tbUsername.Text;

                if (!username.Contains(' ') && username.Length > 5)
                {
                    label1.Hide();
                    tbUsername.Hide();
                    btnColor.Show();
                    btnStart.Hide();
                    lbUsername.Hide();
                    label2.Hide();
                    label3.Hide();
                    pictureBox1.Hide();
                    btnContinue.Show();
                    btnBack.Text = "<-Back";
                    btnBack.Show();
                    this.ReturnValue2 = username;
                    usernameSetted = true;
                }
                else
                    MessageBox.Show("Digitare un Username!(Senza spazi ' ' e con piu di 5 caratteri)");
            }
            else if (usernameSetted && !colorSetted)
            {
                if (ReturnValue1 != new Color())
                {
                    lbUsername.Text = ReturnValue2;
                    pictureBox1.BackColor = ReturnValue1;
                    label1.Hide();
                    tbUsername.Hide();
                    btnColor.Hide();
                    btnStart.Show();
                    lbUsername.Show();
                    label2.Show();
                    label3.Show();
                    pictureBox1.Show();
                    btnContinue.Hide();
                    btnBack.Show();
                    colorSetted = true;
                }
                else
                    MessageBox.Show("Selezionare un colore!");
            }


            }

        private void btnBack_Click(object sender, EventArgs e)
        {
            if (usernameSetted && colorSetted)
            {
                label1.Hide();
                tbUsername.Hide();
                btnColor.Show();
                btnStart.Hide();
                lbUsername.Hide();
                label2.Hide();
                label3.Hide();
                pictureBox1.Hide();
                btnContinue.Show();
                btnBack.Show();
                colorSetted = false;
            }
            else if (usernameSetted)
            {
                label1.Show();
                tbUsername.Show();
                btnColor.Hide();
                btnStart.Hide();
                lbUsername.Hide();
                label2.Hide();
                label3.Hide();
                pictureBox1.Hide();
                btnContinue.Show();
                btnBack.Text = "<-Esci";
                btnBack.Show();
                colorSetted = false;
                usernameSetted = false;
            }
            else
            {
                this.Close();
            }
        }
    }
}
