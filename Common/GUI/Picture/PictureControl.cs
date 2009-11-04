using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine;
using OgreWrapper;
using OgrePlugin;
using System.Drawing.Imaging;
using Medical.Controller;

namespace Medical.GUI
{
    public partial class PictureControl : GUIElement
    {
        private ImageRenderer imageRenderer;
        private DrawingWindowController drawingWindowController;

        public PictureControl()
        {
            InitializeComponent();
        }

        public void initialize(ImageRenderer imageRenderer, DrawingWindowController drawingWindowController)
        {
            this.imageRenderer = imageRenderer;
            this.drawingWindowController = drawingWindowController;
        }

        public void createShortcuts(ShortcutController shortcuts)
        {
            ShortcutGroup group = shortcuts.createOrRetrieveGroup("RenderingShortcuts");

            ShortcutEventCommand render = new ShortcutEventCommand("Render", Keys.F11, false);
            render.Execute += new ShortcutEventCommand.ExecuteEvent(render_Execute);
            group.addShortcut(render);
        }

        private void render_Execute(ShortcutEventCommand shortcut)
        {
            renderSingleButton_Click(null, null);
        }

        private void renderSingleButton_Click(object sender, EventArgs e)
        {
            DrawingWindowHost drawingWindow = drawingWindowController.getActiveWindow();
            if (drawingWindow != null)
            {
                int width = (int)resolutionWidth.Value;
                int height = (int)resolutionHeight.Value;
                Bitmap bitmap = imageRenderer.renderImage(width, height);
                if (bitmap != null)
                {
                    PictureWindow picture = new PictureWindow();
                    picture.initialize(bitmap);
                    picture.Text = String.Format("{0} - {1}x{2}", drawingWindow.Text, width, height);
                    //--UNCOMMENT picture.Show(DockPanel, DockState.Float);
                }
            }
        }

        private void megapixel1dot3_CheckedChanged(object sender, EventArgs e)
        {
            if (megapixel1dot3.Checked)
            {
                resolutionWidth.Value = 1280;
                resolutionHeight.Value = 1024;
            }
        }

        private void megapixel4_CheckedChanged(object sender, EventArgs e)
        {
            if (megapixel4.Checked)
            {
                resolutionWidth.Value = 2448;
                resolutionHeight.Value = 1632;
            }
        }

        private void megapixel6_CheckedChanged(object sender, EventArgs e)
        {
            if (megapixel6.Checked)
            {
                resolutionWidth.Value = 3000;
                resolutionHeight.Value = 2000;
            }
        }

        private void megapixel8_CheckedChanged(object sender, EventArgs e)
        {
            if (megapixel8.Checked)
            {
                resolutionWidth.Value = 3456;
                resolutionHeight.Value = 2304;
            }
        }

        private void megapixel10_CheckedChanged(object sender, EventArgs e)
        {
            if (megapixel10.Checked)
            {
                resolutionWidth.Value = 3648;
                resolutionHeight.Value = 2736;
            }
        }

        private void megapixel12_CheckedChanged(object sender, EventArgs e)
        {
            if (megapixel12.Checked)
            {
                resolutionWidth.Value = 4000;
                resolutionHeight.Value = 3000;
            }
        }

        private void custom_CheckedChanged(object sender, EventArgs e)
        {
            resolutionWidth.Enabled = custom.Checked;
            resolutionHeight.Enabled = custom.Checked;
        }
    }
}
