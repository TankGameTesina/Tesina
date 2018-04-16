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
            this.label1 = new System.Windows.Forms.Label();
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
            this.MainDrawingArea.BackColor = System.Drawing.Color.Maroon;
            this.MainDrawingArea.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainDrawingArea.Location = new System.Drawing.Point(0, 0);
            this.MainDrawingArea.Name = "MainDrawingArea";
            this.MainDrawingArea.Size = new System.Drawing.Size(949, 507);
            this.MainDrawingArea.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.MainDrawingArea.TabIndex = 3;
            this.MainDrawingArea.TabStop = false;
            this.MainDrawingArea.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MainDrawingArea_MouseClick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.SystemColors.ControlLight;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "label1";
            // 
            // FormGame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(949, 507);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MainDrawingArea);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "FormGame";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.MainDrawingArea)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Timer tmrSparo;
        private System.Windows.Forms.PictureBox MainDrawingArea;
        private System.Windows.Forms.Label label1;
    }
}

