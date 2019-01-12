namespace DeploymentToolkit.TrayApp
{
    partial class CloseApplication
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
            this.ListViewCloseApplications = new System.Windows.Forms.ListView();
            this.LabelBottom = new System.Windows.Forms.Label();
            this.ButtonClose = new System.Windows.Forms.Button();
            this.ButtonContinue = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.PanelLoading = new System.Windows.Forms.Panel();
            this.LabelLoading = new System.Windows.Forms.Label();
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
            this.LabelTop.Text = "The following programs must be closed before the installation can proceed.\r\n\r\nPle" +
    "ase save your work, close the programs, and then continue.\r\nAlternatively, save " +
    "your work and click \"Close Programs\".";
            this.LabelTop.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ListViewCloseApplications
            // 
            this.ListViewCloseApplications.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListViewCloseApplications.Location = new System.Drawing.Point(69, 145);
            this.ListViewCloseApplications.Margin = new System.Windows.Forms.Padding(60, 3, 60, 3);
            this.ListViewCloseApplications.Name = "ListViewCloseApplications";
            this.ListViewCloseApplications.Size = new System.Drawing.Size(346, 97);
            this.ListViewCloseApplications.TabIndex = 6;
            this.ListViewCloseApplications.UseCompatibleStateImageBehavior = false;
            this.ListViewCloseApplications.View = System.Windows.Forms.View.List;
            // 
            // LabelBottom
            // 
            this.LabelBottom.Location = new System.Drawing.Point(12, 255);
            this.LabelBottom.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.LabelBottom.Name = "LabelBottom";
            this.LabelBottom.Size = new System.Drawing.Size(460, 57);
            this.LabelBottom.TabIndex = 7;
            this.LabelBottom.Text = "The following programs must be closed before the installation can proceed.\r\n\r\nPle" +
    "ase save your work, close the programs, and then continue.\r\nAlternatively, save " +
    "your work and click \"Close Programs\".";
            this.LabelBottom.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ButtonClose
            // 
            this.ButtonClose.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ButtonClose.Location = new System.Drawing.Point(16, 3);
            this.ButtonClose.Name = "ButtonClose";
            this.ButtonClose.Size = new System.Drawing.Size(120, 24);
            this.ButtonClose.TabIndex = 8;
            this.ButtonClose.Text = "Close Programs";
            this.ButtonClose.UseVisualStyleBackColor = true;
            this.ButtonClose.Click += new System.EventHandler(this.OnButtonCloseClick);
            // 
            // ButtonContinue
            // 
            this.ButtonContinue.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.ButtonContinue.Location = new System.Drawing.Point(323, 3);
            this.ButtonContinue.Name = "ButtonContinue";
            this.ButtonContinue.Size = new System.Drawing.Size(120, 24);
            this.ButtonContinue.TabIndex = 9;
            this.ButtonContinue.Text = "Continue";
            this.ButtonContinue.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.Controls.Add(this.ButtonClose, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ButtonContinue, 2, 0);
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
            this.PanelLoading.Controls.Add(this.LabelLoading);
            this.PanelLoading.Dock = System.Windows.Forms.DockStyle.Fill;
            this.PanelLoading.Location = new System.Drawing.Point(0, 0);
            this.PanelLoading.Name = "PanelLoading";
            this.PanelLoading.Size = new System.Drawing.Size(484, 371);
            this.PanelLoading.TabIndex = 12;
            // 
            // LabelLoading
            // 
            this.LabelLoading.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LabelLoading.Font = new System.Drawing.Font("Arial Narrow", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LabelLoading.Location = new System.Drawing.Point(12, 12);
            this.LabelLoading.Name = "LabelLoading";
            this.LabelLoading.Size = new System.Drawing.Size(460, 350);
            this.LabelLoading.TabIndex = 0;
            this.LabelLoading.Text = "Closing programs...";
            this.LabelLoading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CloseApplication
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 371);
            this.ControlBox = false;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.LabelBottom);
            this.Controls.Add(this.ListViewCloseApplications);
            this.Controls.Add(this.LabelTop);
            this.Controls.Add(this.PictureLogo);
            this.Controls.Add(this.PanelLoading);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(500, 410);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(500, 410);
            this.Name = "CloseApplication";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CloseApplication";
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
        private System.Windows.Forms.ListView ListViewCloseApplications;
        private System.Windows.Forms.Label LabelBottom;
        private System.Windows.Forms.Button ButtonClose;
        private System.Windows.Forms.Button ButtonContinue;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel PanelLoading;
        private System.Windows.Forms.Label LabelLoading;
    }
}