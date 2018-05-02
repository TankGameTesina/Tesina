namespace TestTesinaTrasparenza
{
    partial class Form1
    {
        /// <summary>
        /// Variabile di progettazione necessaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Pulire le risorse in uso.
        /// </summary>
        /// <param name="disposing">ha valore true se le risorse gestite devono essere eliminate, false in caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Codice generato da Progettazione Windows Form

        /// <summary>
        /// Metodo necessario per il supporto della finestra di progettazione. Non modificare
        /// il contenuto del metodo con l'editor di codice.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.picTank = new TestTesinaTrasparenza.TransparentPictureBox();
            this.tmrMovimento = new System.Windows.Forms.Timer(this.components);
            this.picEnemy = new TestTesinaTrasparenza.TransparentPictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tmrSparo = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // picTank
            // 
            this.picTank.Image = null;
            this.picTank.Location = new System.Drawing.Point(54, 104);
            this.picTank.Name = "picTank";
            this.picTank.Size = new System.Drawing.Size(100, 100);
            this.picTank.TabIndex = 0;
            this.picTank.Text = "transparentPictureBox1";
            // 
            // tmrMovimento
            // 
            this.tmrMovimento.Enabled = true;
            this.tmrMovimento.Interval = 1;
            this.tmrMovimento.Tick += new System.EventHandler(this.tmrMovimento_Tick);
            // 
            // picEnemy
            // 
            this.picEnemy.Image = null;
            this.picEnemy.Location = new System.Drawing.Point(222, 58);
            this.picEnemy.Name = "picEnemy";
            this.picEnemy.Size = new System.Drawing.Size(100, 100);
            this.picEnemy.TabIndex = 1;
            this.picEnemy.Text = "transparentPictureBox1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "label1";
            // 
            // tmrSparo
            // 
            this.tmrSparo.Interval = 1;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(396, 346);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.picEnemy);
            this.Controls.Add(this.picTank);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "Form1";
            this.Text = "Form1";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mouseClick);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TransparentPictureBox picTank;
        private System.Windows.Forms.Timer tmrMovimento;
        private TransparentPictureBox picEnemy;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Timer tmrSparo;
    }
}

