using DeploymentToolkit.Modals;
using DeploymentToolkit.TrayApp.Extensions;
using NLog;
using System;
using System.Collections.Generic;
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

            _logger.Trace("Preparing table...");
            for (int i = 0; i < AppView.Columns.Count; i++)
            {
                AppView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            // The name column should use the remaining empty space
            AppView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            this.AddLogo(PictureLogo);
        }

        protected override void SetVisibleCore(bool value)
        {
            if (Program.StartUp)
            {
                Program.StartUp = false;
                base.SetVisibleCore(false);
                return;
            }
            base.SetVisibleCore(value);
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
