namespace HotelAutomationdotnet
{
    partial class CheckoutPanel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CheckoutPanel));
            this.nameBox2 = new System.Windows.Forms.TextBox();
            this.roomNumberBox2 = new System.Windows.Forms.TextBox();
            this.odaKontrolButton = new System.Windows.Forms.Button();
            this.rezervasyonuİptalEtButton = new System.Windows.Forms.Button();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // nameBox2
            // 
            this.nameBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.nameBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.nameBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(208)))), ((int)(((byte)(175)))));
            this.nameBox2.Location = new System.Drawing.Point(62, 65);
            this.nameBox2.Margin = new System.Windows.Forms.Padding(4);
            this.nameBox2.Name = "nameBox2";
            this.nameBox2.Size = new System.Drawing.Size(132, 15);
            this.nameBox2.TabIndex = 0;
            this.nameBox2.Text = "İsim Soyisim";
            this.nameBox2.Click += new System.EventHandler(this.nameBox2_Click);
            // 
            // roomNumberBox2
            // 
            this.roomNumberBox2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.roomNumberBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.roomNumberBox2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(208)))), ((int)(((byte)(175)))));
            this.roomNumberBox2.Location = new System.Drawing.Point(62, 116);
            this.roomNumberBox2.Margin = new System.Windows.Forms.Padding(4);
            this.roomNumberBox2.Name = "roomNumberBox2";
            this.roomNumberBox2.Size = new System.Drawing.Size(132, 15);
            this.roomNumberBox2.TabIndex = 1;
            this.roomNumberBox2.Text = "Oda Numarası";
            this.roomNumberBox2.Click += new System.EventHandler(this.roomNumberBox2_Click);
            // 
            // odaKontrolButton
            // 
            this.odaKontrolButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.odaKontrolButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.odaKontrolButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(208)))), ((int)(((byte)(175)))));
            this.odaKontrolButton.Location = new System.Drawing.Point(13, 313);
            this.odaKontrolButton.Margin = new System.Windows.Forms.Padding(4);
            this.odaKontrolButton.Name = "odaKontrolButton";
            this.odaKontrolButton.Size = new System.Drawing.Size(233, 41);
            this.odaKontrolButton.TabIndex = 3;
            this.odaKontrolButton.Text = "Oda Kontrol";
            this.odaKontrolButton.UseVisualStyleBackColor = false;
            this.odaKontrolButton.Click += new System.EventHandler(this.odaKontrolButton_Click);
            // 
            // rezervasyonuİptalEtButton
            // 
            this.rezervasyonuİptalEtButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.rezervasyonuİptalEtButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.rezervasyonuİptalEtButton.Enabled = false;
            this.rezervasyonuİptalEtButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.rezervasyonuİptalEtButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(208)))), ((int)(((byte)(175)))));
            this.rezervasyonuİptalEtButton.Location = new System.Drawing.Point(541, 313);
            this.rezervasyonuİptalEtButton.Margin = new System.Windows.Forms.Padding(4);
            this.rezervasyonuİptalEtButton.Name = "rezervasyonuİptalEtButton";
            this.rezervasyonuİptalEtButton.Size = new System.Drawing.Size(233, 46);
            this.rezervasyonuİptalEtButton.TabIndex = 4;
            this.rezervasyonuİptalEtButton.Text = "Rezervasyonu İptal Et";
            this.rezervasyonuİptalEtButton.UseVisualStyleBackColor = false;
            this.rezervasyonuİptalEtButton.Click += new System.EventHandler(this.rezervasyonuİptalEtButton_Click);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CalendarMonthBackground = System.Drawing.Color.White;
            this.dateTimePicker1.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.dateTimePicker1.Location = new System.Drawing.Point(145, 159);
            this.dateTimePicker1.MinDate = new System.DateTime(2024, 9, 24, 0, 0, 0, 0);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(247, 22);
            this.dateTimePicker1.TabIndex = 5;
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.CalendarMonthBackground = System.Drawing.Color.White;
            this.dateTimePicker2.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.dateTimePicker2.Location = new System.Drawing.Point(145, 187);
            this.dateTimePicker2.MinDate = new System.DateTime(2024, 9, 24, 0, 0, 0, 0);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(247, 22);
            this.dateTimePicker2.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(208)))), ((int)(((byte)(175)))));
            this.label1.Location = new System.Drawing.Point(59, 159);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 16);
            this.label1.TabIndex = 8;
            this.label1.Text = "Giriş Tarihi";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(55)))), ((int)(((byte)(65)))));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(208)))), ((int)(((byte)(175)))));
            this.label2.Location = new System.Drawing.Point(59, 193);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 16);
            this.label2.TabIndex = 9;
            this.label2.Text = "Çıkış Tarihi";
            // 
            // CheckoutPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(140)))), ((int)(((byte)(129)))), ((int)(((byte)(106)))));
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(787, 372);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.rezervasyonuİptalEtButton);
            this.Controls.Add(this.odaKontrolButton);
            this.Controls.Add(this.roomNumberBox2);
            this.Controls.Add(this.nameBox2);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "CheckoutPanel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CheckoutPanel";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox nameBox2;
        private System.Windows.Forms.TextBox roomNumberBox2;
        private System.Windows.Forms.Button odaKontrolButton;
        private System.Windows.Forms.Button rezervasyonuİptalEtButton;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}