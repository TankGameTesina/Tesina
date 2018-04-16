namespace MainGame
{
    partial class FormGame
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
            this.tmrSparo = new System.Windows.Forms.Timer(this.components);
            this.MainDrawingArea = new System.Windows.Forms.PictureBox();
            this.listBox1 = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.MainDrawingArea)).BeginInit();
            this.SuspendLayout();
            // 
            // tmrSparo
            // 
            this.tmrSparo.Enabled = true;
            this.tmrSparo.Interval = 10;
            this.tmrSparo.Tick += new System.EventHandler(this.tmrSparo_Tick);
            // 
            // MainDrawingArea
            // 
            this.MainDrawingArea.BackColor = System.Drawing.Color.Gainsboro;
            this.MainDrawingArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainDrawingArea.Location = new System.Drawing.Point(0, 0);
            this.MainDrawingArea.Name = "MainDrawingArea";
            this.MainDrawingArea.Size = new System.Drawing.Size(949, 507);
            this.MainDrawingArea.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.MainDrawingArea.TabIndex = 3;
            this.MainDrawingArea.TabStop = false;
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(586, 3);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(363, 498);
            this.listBox1.TabIndex = 4;
            // 
            // FormGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(949, 507);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.MainDrawingArea);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormGame";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.MainDrawingArea)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer tmrSparo;
        private System.Windows.Forms.PictureBox MainDrawingArea;
        private System.Windows.Forms.ListBox listBox1;
    }
}

