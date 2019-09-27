namespace ThermostatControler
{
    partial class ShowSchedule
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
            this.pnlMon = new System.Windows.Forms.Panel();
            this.cBoxSchedules = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.pnlTue = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.pnlWed = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.pnlThu = new System.Windows.Forms.Panel();
            this.label5 = new System.Windows.Forms.Label();
            this.pnlfri = new System.Windows.Forms.Panel();
            this.label6 = new System.Windows.Forms.Label();
            this.pnlSat = new System.Windows.Forms.Panel();
            this.pnlSun = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblPickDay = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // pnlMon
            // 
            this.pnlMon.Location = new System.Drawing.Point(17, 65);
            this.pnlMon.Name = "pnlMon";
            this.pnlMon.Size = new System.Drawing.Size(144, 100);
            this.pnlMon.TabIndex = 0;
            this.pnlMon.Click += new System.EventHandler(this.pnlMon_Click);
            // 
            // cBoxSchedules
            // 
            this.cBoxSchedules.FormattingEnabled = true;
            this.cBoxSchedules.Location = new System.Drawing.Point(120, 7);
            this.cBoxSchedules.Name = "cBoxSchedules";
            this.cBoxSchedules.Size = new System.Drawing.Size(140, 23);
            this.cBoxSchedules.TabIndex = 1;
            this.cBoxSchedules.SelectedIndexChanged += new System.EventHandler(this.cBoxSchedules_SelectedIndexChanged);
            this.cBoxSchedules.TextUpdate += new System.EventHandler(this.cBoxSchedules_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select Schedule";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 47);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "Monday";
            // 
            // pnlTue
            // 
            this.pnlTue.Location = new System.Drawing.Point(212, 65);
            this.pnlTue.Name = "pnlTue";
            this.pnlTue.Size = new System.Drawing.Size(144, 100);
            this.pnlTue.TabIndex = 0;
            this.pnlTue.Click += new System.EventHandler(this.pnlTue_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(209, 47);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "Tuesday";
            // 
            // pnlWed
            // 
            this.pnlWed.Location = new System.Drawing.Point(407, 65);
            this.pnlWed.Name = "pnlWed";
            this.pnlWed.Size = new System.Drawing.Size(144, 100);
            this.pnlWed.TabIndex = 0;
            this.pnlWed.Click += new System.EventHandler(this.pnlWed_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(404, 47);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 15);
            this.label4.TabIndex = 2;
            this.label4.Text = "Wednesday";
            // 
            // pnlThu
            // 
            this.pnlThu.Location = new System.Drawing.Point(20, 199);
            this.pnlThu.Name = "pnlThu";
            this.pnlThu.Size = new System.Drawing.Size(144, 100);
            this.pnlThu.TabIndex = 0;
            this.pnlThu.Click += new System.EventHandler(this.pnlThu_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(17, 181);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(57, 15);
            this.label5.TabIndex = 2;
            this.label5.Text = "Thursday";
            // 
            // pnlfri
            // 
            this.pnlfri.Location = new System.Drawing.Point(212, 199);
            this.pnlfri.Name = "pnlfri";
            this.pnlfri.Size = new System.Drawing.Size(144, 100);
            this.pnlfri.TabIndex = 0;
            this.pnlfri.Click += new System.EventHandler(this.pnlfri_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(209, 181);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 15);
            this.label6.TabIndex = 2;
            this.label6.Text = "Friday";
            // 
            // pnlSat
            // 
            this.pnlSat.Location = new System.Drawing.Point(407, 199);
            this.pnlSat.Name = "pnlSat";
            this.pnlSat.Size = new System.Drawing.Size(144, 100);
            this.pnlSat.TabIndex = 0;
            this.pnlSat.Click += new System.EventHandler(this.pnlSat_Click);
            // 
            // pnlSun
            // 
            this.pnlSun.Location = new System.Drawing.Point(20, 329);
            this.pnlSun.Name = "pnlSun";
            this.pnlSun.Size = new System.Drawing.Size(144, 100);
            this.pnlSun.TabIndex = 0;
            this.pnlSun.Click += new System.EventHandler(this.pnlSun_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(407, 181);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 15);
            this.label7.TabIndex = 2;
            this.label7.Text = "Saturday";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 311);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(48, 15);
            this.label8.TabIndex = 2;
            this.label8.Text = "Sunday";
            // 
            // lblPickDay
            // 
            this.lblPickDay.AutoSize = true;
            this.lblPickDay.Location = new System.Drawing.Point(267, 10);
            this.lblPickDay.Name = "lblPickDay";
            this.lblPickDay.Size = new System.Drawing.Size(75, 15);
            this.lblPickDay.TabIndex = 3;
            this.lblPickDay.Text = "and pick day";
            // 
            // ShowSchedule
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(582, 451);
            this.Controls.Add(this.lblPickDay);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.pnlSun);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.pnlfri);
            this.Controls.Add(this.pnlSat);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.pnlThu);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.pnlWed);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pnlTue);
            this.Controls.Add(this.cBoxSchedules);
            this.Controls.Add(this.pnlMon);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.Name = "ShowSchedule";
            this.Text = "ShowSchedule";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel pnlMon;
        private System.Windows.Forms.ComboBox cBoxSchedules;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel pnlTue;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel pnlWed;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel pnlThu;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Panel pnlfri;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel pnlSat;
        private System.Windows.Forms.Panel pnlSun;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblPickDay;
    }
}