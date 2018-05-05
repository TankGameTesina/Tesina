namespace ServerChoosingDemo
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
            this.lbDebug = new System.Windows.Forms.ListBox();
            this.btnLooking = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbPkt = new System.Windows.Forms.Label();
            this.lbInvio = new System.Windows.Forms.Label();
            this.lbServer = new System.Windows.Forms.Label();
            this.lbRicerca = new System.Windows.Forms.Label();
            this.tmrServer = new System.Windows.Forms.Timer(this.components);
            this.tmrInvio = new System.Windows.Forms.Timer(this.components);
            this.lbRead = new System.Windows.Forms.Label();
            this.lbWrite = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cbLocal = new System.Windows.Forms.CheckBox();
            this.lbDemo = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbDebug
            // 
            this.lbDebug.FormattingEnabled = true;
            this.lbDebug.Location = new System.Drawing.Point(13, 35);
            this.lbDebug.Name = "lbDebug";
            this.lbDebug.Size = new System.Drawing.Size(332, 303);
            this.lbDebug.TabIndex = 0;
            // 
            // btnLooking
            // 
            this.btnLooking.Location = new System.Drawing.Point(352, 35);
            this.btnLooking.Name = "btnLooking";
            this.btnLooking.Size = new System.Drawing.Size(135, 23);
            this.btnLooking.TabIndex = 1;
            this.btnLooking.Text = "Incomincia a cercare server";
            this.btnLooking.UseVisualStyleBackColor = true;
            this.btnLooking.Click += new System.EventHandler(this.btnLooking_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(352, 89);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(135, 23);
            this.btnSend.TabIndex = 2;
            this.btnSend.Text = "Inizia ad inviare";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lbDemo);
            this.groupBox1.Controls.Add(this.lbPkt);
            this.groupBox1.Controls.Add(this.lbInvio);
            this.groupBox1.Controls.Add(this.lbServer);
            this.groupBox1.Location = new System.Drawing.Point(352, 143);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(135, 114);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Status";
            // 
            // lbPkt
            // 
            this.lbPkt.AutoSize = true;
            this.lbPkt.Location = new System.Drawing.Point(7, 61);
            this.lbPkt.Name = "lbPkt";
            this.lbPkt.Size = new System.Drawing.Size(35, 13);
            this.lbPkt.TabIndex = 2;
            this.lbPkt.Text = "label3";
            // 
            // lbInvio
            // 
            this.lbInvio.AutoSize = true;
            this.lbInvio.Location = new System.Drawing.Point(7, 41);
            this.lbInvio.Name = "lbInvio";
            this.lbInvio.Size = new System.Drawing.Size(35, 13);
            this.lbInvio.TabIndex = 1;
            this.lbInvio.Text = "label2";
            // 
            // lbServer
            // 
            this.lbServer.AutoSize = true;
            this.lbServer.Location = new System.Drawing.Point(7, 20);
            this.lbServer.Name = "lbServer";
            this.lbServer.Size = new System.Drawing.Size(35, 13);
            this.lbServer.TabIndex = 0;
            this.lbServer.Text = "label1";
            // 
            // lbRicerca
            // 
            this.lbRicerca.AutoSize = true;
            this.lbRicerca.Location = new System.Drawing.Point(351, 66);
            this.lbRicerca.Name = "lbRicerca";
            this.lbRicerca.Size = new System.Drawing.Size(52, 13);
            this.lbRicerca.TabIndex = 5;
            this.lbRicerca.Text = "lbRicerca";
            // 
            // tmrServer
            // 
            this.tmrServer.Interval = 500;
            this.tmrServer.Tick += new System.EventHandler(this.tmrServer_Tick);
            // 
            // tmrInvio
            // 
            this.tmrInvio.Interval = 10;
            this.tmrInvio.Tick += new System.EventHandler(this.tmrInvio_Tick);
            // 
            // lbRead
            // 
            this.lbRead.AutoSize = true;
            this.lbRead.Location = new System.Drawing.Point(362, 260);
            this.lbRead.Name = "lbRead";
            this.lbRead.Size = new System.Drawing.Size(35, 13);
            this.lbRead.TabIndex = 6;
            this.lbRead.Text = "label1";
            // 
            // lbWrite
            // 
            this.lbWrite.AutoSize = true;
            this.lbWrite.Location = new System.Drawing.Point(362, 277);
            this.lbWrite.Name = "lbWrite";
            this.lbWrite.Size = new System.Drawing.Size(35, 13);
            this.lbWrite.TabIndex = 7;
            this.lbWrite.Text = "label1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Testing";
            // 
            // cbLocal
            // 
            this.cbLocal.AutoSize = true;
            this.cbLocal.Location = new System.Drawing.Point(57, 13);
            this.cbLocal.Name = "cbLocal";
            this.cbLocal.Size = new System.Drawing.Size(54, 17);
            this.cbLocal.TabIndex = 9;
            this.cbLocal.Text = "locale";
            this.cbLocal.UseVisualStyleBackColor = true;
            this.cbLocal.CheckedChanged += new System.EventHandler(this.cbLocal_CheckedChanged);
            // 
            // lbDemo
            // 
            this.lbDemo.AutoSize = true;
            this.lbDemo.Location = new System.Drawing.Point(7, 83);
            this.lbDemo.Name = "lbDemo";
            this.lbDemo.Size = new System.Drawing.Size(35, 13);
            this.lbDemo.TabIndex = 3;
            this.lbDemo.Text = "label2";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(498, 357);
            this.Controls.Add(this.cbLocal);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbWrite);
            this.Controls.Add(this.lbRead);
            this.Controls.Add(this.lbRicerca);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnLooking);
            this.Controls.Add(this.lbDebug);
            this.Name = "Form1";
            this.Text = "ServerDemo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox lbDebug;
        private System.Windows.Forms.Button btnLooking;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbPkt;
        private System.Windows.Forms.Label lbInvio;
        private System.Windows.Forms.Label lbServer;
        private System.Windows.Forms.Label lbRicerca;
        private System.Windows.Forms.Timer tmrServer;
        private System.Windows.Forms.Timer tmrInvio;
        private System.Windows.Forms.Label lbRead;
        private System.Windows.Forms.Label lbWrite;
        private System.Windows.Forms.Label lbDemo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbLocal;
    }
}

