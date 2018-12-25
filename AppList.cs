using DeploymentToolkit.Modals;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DeploymentToolkit.TrayApp
{
    public partial class AppList : Form
    {
        public BindingSource BindingSource;

        private List<App> _apps = new List<App>();

        public AppList()
        {
            InitializeComponent();

            BindingSource = new BindingSource() { DataSource = _apps };
            AppView.DataSource = BindingSource;

            for (int i = 0; i < AppView.Columns.Count; i++)
            {
                AppView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            // The name column should use the remaining empty space
            AppView.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
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
