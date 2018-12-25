namespace DeploymentToolkit.TrayApp
{
    partial class AppList
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
            this.AppView = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ButtonInstallAll = new System.Windows.Forms.Button();
            this.ButtonInstallSelected = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.AppView)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // AppView
            // 
            this.AppView.AllowUserToAddRows = false;
            this.AppView.AllowUserToDeleteRows = false;
            this.AppView.AllowUserToOrderColumns = true;
            this.AppView.AllowUserToResizeRows = false;
            this.AppView.BackgroundColor = System.Drawing.Color.White;
            this.AppView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.AppView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.AppView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AppView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.AppView.Location = new System.Drawing.Point(3, 21);
            this.AppView.Name = "AppView";
            this.AppView.RowHeadersVisible = false;
            this.AppView.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.AppView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.AppView.Size = new System.Drawing.Size(854, 241);
            this.AppView.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.AppView);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(860, 265);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Apps";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.ButtonInstallAll);
            this.groupBox2.Controls.Add(this.ButtonInstallSelected);
            this.groupBox2.Location = new System.Drawing.Point(15, 283);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(854, 66);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options";
            // 
            // ButtonInstallAll
            // 
            this.ButtonInstallAll.Location = new System.Drawing.Point(158, 24);
            this.ButtonInstallAll.Name = "ButtonInstallAll";
            this.ButtonInstallAll.Size = new System.Drawing.Size(146, 34);
            this.ButtonInstallAll.TabIndex = 1;
            this.ButtonInstallAll.Text = "Install all";
            this.ButtonInstallAll.UseVisualStyleBackColor = true;
            // 
            // ButtonInstallSelected
            // 
            this.ButtonInstallSelected.Location = new System.Drawing.Point(6, 24);
            this.ButtonInstallSelected.Name = "ButtonInstallSelected";
            this.ButtonInstallSelected.Size = new System.Drawing.Size(146, 34);
            this.ButtonInstallSelected.TabIndex = 0;
            this.ButtonInstallSelected.Text = "Install selected";
            this.ButtonInstallSelected.UseVisualStyleBackColor = true;
            // 
            // AppList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 361);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Arial Narrow", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.MinimumSize = new System.Drawing.Size(900, 400);
            this.Name = "AppList";
            this.ShowIcon = false;
            this.Text = "DeploymentToolkit TrayApp";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.AppList_FormClosing);
            this.Load += new System.EventHandler(this.AppList_Load);
            ((System.ComponentModel.ISupportInitialize)(this.AppView)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView AppView;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button ButtonInstallAll;
        private System.Windows.Forms.Button ButtonInstallSelected;
    }
}

