namespace GraficaTesina
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
            this.tbPath = new System.Windows.Forms.TextBox();
            this.btnConferma = new System.Windows.Forms.Button();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.btnPath = new System.Windows.Forms.Button();
            this.btnColore = new System.Windows.Forms.Button();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.picColore = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picColore)).BeginInit();
            this.SuspendLayout();
            // 
            // tbPath
            // 
            this.tbPath.Location = new System.Drawing.Point(36, 24);
            this.tbPath.Name = "tbPath";
            this.tbPath.Size = new System.Drawing.Size(370, 20);
            this.tbPath.TabIndex = 0;
            // 
            // btnConferma
            // 
            this.btnConferma.Location = new System.Drawing.Point(370, 79);
            this.btnConferma.Name = "btnConferma";
            this.btnConferma.Size = new System.Drawing.Size(75, 23);
            this.btnConferma.TabIndex = 1;
            this.btnConferma.Text = "Conferma";
            this.btnConferma.UseVisualStyleBackColor = true;
            this.btnConferma.Click += new System.EventHandler(this.btnConferma_Click);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Location = new System.Drawing.Point(211, 50);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(50, 50);
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // btnPath
            // 
            this.btnPath.Location = new System.Drawing.Point(413, 24);
            this.btnPath.Name = "btnPath";
            this.btnPath.Size = new System.Drawing.Size(32, 20);
            this.btnPath.TabIndex = 3;
            this.btnPath.Text = "...";
            this.btnPath.UseVisualStyleBackColor = true;
            this.btnPath.Click += new System.EventHandler(this.btnPath_Click);
            // 
            // btnColore
            // 
            this.btnColore.Location = new System.Drawing.Point(36, 50);
            this.btnColore.Name = "btnColore";
            this.btnColore.Size = new System.Drawing.Size(126, 23);
            this.btnColore.TabIndex = 4;
            this.btnColore.Text = "Scegli Colore";
            this.btnColore.UseVisualStyleBackColor = true;
            this.btnColore.Click += new System.EventHandler(this.btnColore_Click);
            // 
            // picColore
            // 
            this.picColore.Location = new System.Drawing.Point(36, 79);
            this.picColore.Name = "picColore";
            this.picColore.Size = new System.Drawing.Size(126, 21);
            this.picColore.TabIndex = 5;
            this.picColore.TabStop = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(457, 115);
            this.Controls.Add(this.picColore);
            this.Controls.Add(this.btnColore);
            this.Controls.Add(this.btnPath);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.btnConferma);
            this.Controls.Add(this.tbPath);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picColore)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tbPath;
        private System.Windows.Forms.Button btnConferma;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button btnPath;
        private System.Windows.Forms.Button btnColore;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.PictureBox picColore;
    }
}

