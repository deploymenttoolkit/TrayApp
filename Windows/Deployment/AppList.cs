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
        private List<DeferedDeployment> _deferedDeployments = new List<DeferedDeployment>();

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

            this.ApplySettings(PictureLogo);
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

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Visible)
            {
                LoadDeferedDeployments();
            }
            base.OnVisibleChanged(e);
        }

        private void AppList_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Disable closing, just hide
            e.Cancel = true;

            Program.HideAppList();
        }

        private void LoadDeferedDeployments()
        {
            _deferedDeployments = ToolkitEnvironment.RegistryManager.GetAllDeferedDeployments();
            _logger.Trace($"Found {_deferedDeployments.Count} defered deployments");

            BindingSource.Clear();

            for (var i = 0; i < _deferedDeployments.Count; i++)
            {
                var deployment = _deferedDeployments[i];
                BindingSource.Add(new App()
                {
                    ID = i,
                    AppName = deployment.Name,
                    RemainingDays = deployment.RemainingDays != -1 ? deployment.RemainingDays.ToString() : string.Empty,
                    Deadline = deployment.Deadline != DateTime.MinValue ? deployment.Deadline.ToString() : string.Empty
                });
            }
        }
    }
}
