namespace DeploymentToolkit.TrayApp
{
    partial class RestartDialog
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
            this.PictureLogo = new System.Windows.Forms.PictureBox();
            this.LabelTop = new System.Windows.Forms.Label();
            this.LabelBottom = new System.Windows.Forms.Label();
            this.ButtonLater = new System.Windows.Forms.Button();
            this.ButtonNow = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.PanelLoading = new System.Windows.Forms.Panel();
            this.LabelCenter = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.PictureLogo)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.PanelLoading.SuspendLayout();
            this.SuspendLayout();
            // 
            // PictureLogo
            // 
            this.PictureLogo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PictureLogo.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.PictureLogo.ErrorImage = null;
            this.PictureLogo.InitialImage = null;
            this.PictureLogo.Location = new System.Drawing.Point(12, 12);
            this.PictureLogo.Name = "PictureLogo";
            this.PictureLogo.Size = new System.Drawing.Size(460, 50);
            this.PictureLogo.TabIndex = 4;
            this.PictureLogo.TabStop = false;
            // 
            // LabelTop
            // 
            this.LabelTop.Location = new System.Drawing.Point(12, 75);
            this.LabelTop.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.LabelTop.Name = "LabelTop";
            this.LabelTop.Size = new System.Drawing.Size(460, 57);
            this.LabelTop.TabIndex = 5;
            this.LabelTop.Text = "EditMe";
            this.LabelTop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LabelBottom
            // 
            this.LabelBottom.Location = new System.Drawing.Point(12, 255);
            this.LabelBottom.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.LabelBottom.Name = "LabelBottom";
            this.LabelBottom.Size = new System.Drawing.Size(460, 57);
            this.LabelBottom.TabIndex = 7;
            this.LabelBottom.Text = "EditMe";
            this.LabelBottom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ButtonLater
            // 
            this.ButtonLater.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ButtonLater.Location = new System.Drawing.Point(16, 3);
            this.ButtonLater.Name = "ButtonLater";
            this.ButtonLater.Size = new System.Drawing.Size(120, 24);
            this.ButtonLater.TabIndex = 8;
            this.ButtonLater.Text = "EditMe";
            this.ButtonLater.UseVisualStyleBackColor = true;
            this.ButtonLater.Click += new System.EventHandler(this.ButtonLater_Click);
            // 
            // ButtonNow
            // 
            this.ButtonNow.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ButtonNow.Location = new System.Drawing.Point(323, 3);
            this.ButtonNow.Name = "ButtonNow";
            this.ButtonNow.Size = new System.Drawing.Size(120, 24);
            this.ButtonNow.TabIndex = 9;
            this.ButtonNow.Text = "EditMe";
            this.ButtonNow.UseVisualStyleBackColor = true;
            this.ButtonNow.Click += new System.EventHandler(this.ButtonNow_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.ButtonLater, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ButtonNow, 2, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 325);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 10);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(460, 30);
            this.tableLayoutPanel1.TabIndex = 11;
            // 
            // PanelLoading
            // 
            this.PanelLoading.Controls.Add(this.LabelCenter);
            this.PanelLoading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelLoading.Location = new System.Drawing.Point(0, 0);
            this.PanelLoading.Name = "PanelLoading";
            this.PanelLoading.Size = new System.Drawing.Size(484, 371);
            this.PanelLoading.TabIndex = 12;
            // 
            // LabelCenter
            // 
            this.LabelCenter.Location = new System.Drawing.Point(12, 152);
            this.LabelCenter.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.LabelCenter.Name = "LabelCenter";
            this.LabelCenter.Size = new System.Drawing.Size(460, 83);
            this.LabelCenter.TabIndex = 13;
            this.LabelCenter.Text = "EditMe";
            this.LabelCenter.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RestartDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 371);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.LabelBottom);
            this.Controls.Add(this.LabelTop);
            this.Controls.Add(this.PictureLogo);
            this.Controls.Add(this.PanelLoading);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 410);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 410);
            this.Name = "RestartDialog";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RestartDialog";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
            this.Resize += new System.EventHandler(this.OnResize);
            ((System.ComponentModel.ISupportInitialize)(this.PictureLogo)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.PanelLoading.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox PictureLogo;
        private System.Windows.Forms.Label LabelTop;
        private System.Windows.Forms.Label LabelBottom;
        private System.Windows.Forms.Button ButtonLater;
        private System.Windows.Forms.Button ButtonNow;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel PanelLoading;
        private System.Windows.Forms.Label LabelCenter;
    }
}