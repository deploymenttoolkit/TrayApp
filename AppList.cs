using DeploymentToolkit.Modals;
using NLog;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DeploymentToolkit.TrayApp
{
    public partial class AppList : Form
    {
        public BindingSource BindingSource;

        private List<App> _apps = new List<App>();

        private Logger _logger = LogManager.GetCurrentClassLogger();

        public AppList()
        {
            _logger.Trace("Initializing components...");
            InitializeComponent();

            _logger.Trace("Initializing DataSource...");
            BindingSource = new BindingSource() { DataSource = _apps };
            AppView.DataSource = BindingSource;

            _logger.Trace("Preparting table...");
            for (int i = 0; i < AppView.Columns.Count; i++)
            {
                AppView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            // The name column should use the remaining empty space
            AppView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            _logger.Trace("Searching for a Logo.png ...");
            if(File.Exists("Logo.png"))
            {
                try
                {
                    this.PictureLogo.Image = Image.FromFile("Logo.png");

                    /*
                     * We could tecnically implement alignment here
                     * The idea would be for the logo to be able to be left, right or center aligned
                     * Currently the logo is always center aligned which may look good or not
                     * Someone should look into this once things "work"
                     */
                    //if(this.PictureLogo.Image.Size.Height != 100)
                    //{
                    //    var scale = 100f / this.PictureLogo.Image.Height;
                    //    var newWidth = this.PictureLogo.Image.Width * scale;
                    //    var newHeight = this.PictureLogo.Image.Height * scale;
                    //    var image = new Bitmap((int)newWidth, (int)newHeight);
                    //    using (var graphic = Graphics.FromImage(image))
                    //    {
                    //        graphic.DrawImage(this.PictureLogo.Image, 0, 0, newWidth, newHeight);
                    //    }
                    //    this.PictureLogo.Image = image;

                    //    if(image.Width < this.groupBox1.Width)
                    //    {
                    //        this.PictureLogo.Width = image.Width;
                    //        this.PictureLogo.Left = this.groupBox1.Width - image.Width;
                    //        this.PictureLogo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                    //    }
                    //}

                    this.PictureLogo.SizeMode = PictureBoxSizeMode.Zoom;
                }
                catch(Exception ex)
                {
                    _logger.Error(ex, "Failed to create logo");
                }
            }
        }

        private void AppList_Load(object sender, EventArgs e)
        {

            // Testing only
            //for(int i = 0; i < 200; i++)
            //{
            //    BindingSource.Add(new App()
            //    {
            //        ID = $"ID-{i}",
            //        AppName = $"Test-{i}",
            //        DeferTimes = "None",
            //        DeferDays = "None",
            //        DeferDeadline = DateTime.Now.ToLongDateString()
            //    });
            //}
        }

        private void AppList_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Disable closing, just hide
            e.Cancel = true;

            Program.HideAppList();
        }
    }
}
