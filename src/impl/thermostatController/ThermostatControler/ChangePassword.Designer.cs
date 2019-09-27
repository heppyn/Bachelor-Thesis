namespace ThermostatControler
{
    partial class ChangePassword
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtPass2 = new System.Windows.Forms.TextBox();
            this.txtPass1 = new System.Windows.Forms.TextBox();
            this.btnPassOk = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 68);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Repeat password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "New password";
            // 
            // txtPass2
            // 
            this.txtPass2.Location = new System.Drawing.Point(109, 65);
            this.txtPass2.Name = "txtPass2";
            this.txtPass2.Size = new System.Drawing.Size(145, 20);
            this.txtPass2.TabIndex = 1;
            this.txtPass2.UseSystemPasswordChar = true;
            // 
            // txtPass1
            // 
            this.txtPass1.Location = new System.Drawing.Point(109, 39);
            this.txtPass1.Name = "txtPass1";
            this.txtPass1.Size = new System.Drawing.Size(145, 20);
            this.txtPass1.TabIndex = 1;
            this.txtPass1.UseSystemPasswordChar = true;
            // 
            // btnPassOk
            // 
            this.btnPassOk.Location = new System.Drawing.Point(179, 91);
            this.btnPassOk.Name = "btnPassOk";
            this.btnPassOk.Size = new System.Drawing.Size(75, 23);
            this.btnPassOk.TabIndex = 2;
            this.btnPassOk.Text = "OK";
            this.btnPassOk.UseVisualStyleBackColor = true;
            this.btnPassOk.Click += new System.EventHandler(this.btnPassOk_Click);
            // 
            // ChangePassword
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.ClientSize = new System.Drawing.Size(286, 181);
            this.Controls.Add(this.btnPassOk);
            this.Controls.Add(this.txtPass1);
            this.Controls.Add(this.txtPass2);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "ChangePassword";
            this.Text = "Please Change Password";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtPass2;
        private System.Windows.Forms.TextBox txtPass1;
        private System.Windows.Forms.Button btnPassOk;
    }
}