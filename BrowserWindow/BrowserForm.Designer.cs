using System.Drawing;

namespace TCDefectIntegration.BrowserWindow
{
    partial class BrowserForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowserForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.labelRight = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.successPanel = new System.Windows.Forms.Panel();
            this.successButton = new System.Windows.Forms.Button();
            this.successLabel = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.successPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(99)))), ((int)(((byte)(175)))));
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.closeButton);
            this.panel1.Controls.Add(this.labelRight);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.MaximumSize = new System.Drawing.Size(2900, 41);
            this.panel1.MinimumSize = new System.Drawing.Size(1144, 41);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1144, 41);
            this.panel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(3, 4);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(390, 35);
            this.pictureBox1.TabIndex = 7;
            this.pictureBox1.TabStop = false;
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.BackColor = System.Drawing.Color.White;
            this.closeButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(99)))), ((int)(((byte)(175)))));
            this.closeButton.Location = new System.Drawing.Point(1057, 9);
            this.closeButton.MaximumSize = new System.Drawing.Size(75, 25);
            this.closeButton.MinimumSize = new System.Drawing.Size(75, 25);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(75, 25);
            this.closeButton.TabIndex = 6;
            this.closeButton.Text = "Cancel";
            this.closeButton.UseVisualStyleBackColor = false;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // labelRight
            // 
            this.labelRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelRight.AutoSize = true;
            this.labelRight.Font = new System.Drawing.Font("Open Sans", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelRight.ForeColor = System.Drawing.Color.White;
            this.labelRight.Location = new System.Drawing.Point(750, 9);
            this.labelRight.MaximumSize = new System.Drawing.Size(300, 0);
            this.labelRight.MinimumSize = new System.Drawing.Size(300, 0);
            this.labelRight.Name = "labelRight";
            this.labelRight.Size = new System.Drawing.Size(300, 22);
            this.labelRight.TabIndex = 5;
            this.labelRight.Text = "Create your Jira issue.";
            this.labelRight.UseWaitCursor = true;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.Controls.Add(this.successPanel);
            this.panel2.Location = new System.Drawing.Point(0, 47);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1144, 661);
            this.panel2.TabIndex = 1;
            // 
            // successPanel
            // 
            this.successPanel.Controls.Add(this.successButton);
            this.successPanel.Controls.Add(this.successLabel);
            this.successPanel.Location = new System.Drawing.Point(255, 211);
            this.successPanel.Name = "successPanel";
            this.successPanel.Size = new System.Drawing.Size(627, 106);
            this.successPanel.TabIndex = 0;
            this.successPanel.Visible = false;
            // 
            // successButton
            // 
            this.successButton.BackColor = System.Drawing.Color.White;
            this.successButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(29)))), ((int)(((byte)(99)))), ((int)(((byte)(175)))));
            this.successButton.Location = new System.Drawing.Point(527, 33);
            this.successButton.Name = "successButton";
            this.successButton.Size = new System.Drawing.Size(84, 36);
            this.successButton.TabIndex = 8;
            this.successButton.Text = "Return";
            this.successButton.UseVisualStyleBackColor = false;
            this.successButton.Visible = false;
            this.successButton.Click += new System.EventHandler(this.successButton_Click);
            // 
            // successLabel
            // 
            this.successLabel.AutoSize = true;
            this.successLabel.Font = new System.Drawing.Font("Open Sans", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.successLabel.ForeColor = System.Drawing.Color.DarkGreen;
            this.successLabel.Location = new System.Drawing.Point(22, 33);
            this.successLabel.Name = "successLabel";
            this.successLabel.Size = new System.Drawing.Size(482, 37);
            this.successLabel.TabIndex = 0;
            this.successLabel.Text = "Captured Jira Issue, good to return";
            this.successLabel.Visible = false;
            // 
            // BrowserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1144, 708);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BrowserForm";
            this.Text = "Browser";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.successPanel.ResumeLayout(false);
            this.successPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label labelRight;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel successPanel;
        private System.Windows.Forms.Label successLabel;
        private System.Windows.Forms.Button successButton;
    }
}