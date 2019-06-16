using DeploymentToolkit.Util;
using NLog;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DeploymentToolkit.TrayApp.Extensions
{
    public static class FormExtension
    {
        private static Logger _logger = LogManager.GetCurrentClassLogger();

        public static void ApplySettings(this Form form, PictureBox logo)
        {
            ApplyLogo(form, logo);

            var settings = Program.Settings;
            if (!string.IsNullOrEmpty(settings.BackgroundColor))
            {
                var backgroundColor = settings.BackgroundColor;
                _logger.Trace($"Trying to apply {backgroundColor}");

                var color = ColorUtil.GetColor(backgroundColor);
                if (color != default(Color))
                {
                    form.BackColor = color;
                }
            }

            if (!string.IsNullOrEmpty(settings.ForegroundColor))
            {
                var foregroundColor = settings.ForegroundColor;
                _logger.Trace($"Trying to apply {foregroundColor}");

                var color = ColorUtil.GetColor(foregroundColor);
                if (color != default(Color))
                {
                    form.ForeColor = color;
                }
            }
        }

        public static void ApplyLogo(this Form form, PictureBox logo)
        {
            _logger.Trace("Searching for a Logo.png ...");
            if (File.Exists("Logo.png"))
            {
                try
                {
                    logo.Image = Image.FromFile("Logo.png");

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

                    logo.SizeMode = PictureBoxSizeMode.Zoom;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to create logo");
                }
            }
        }
    }
}
