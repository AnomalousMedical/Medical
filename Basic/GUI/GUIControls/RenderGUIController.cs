using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using ComponentFactory.Krypton.Ribbon;
using ComponentFactory.Krypton.Toolkit;
using System.Drawing;
using System.Windows.Forms;

namespace Medical.GUI
{
    class RenderGUIController
    {
        private KryptonRibbonGroupNumericUpDown width;
        private KryptonRibbonGroupNumericUpDown height;
        private BasicForm form;
        private ImageRenderer imageRenderer;
        private DrawingWindowController drawingWindowController;

        private KryptonContextMenu resolutionMenu;
        private KryptonContextMenuRadioButton onePointThreeMegapixel;
        private KryptonContextMenuRadioButton fourMegapixel;
        private KryptonContextMenuRadioButton sixMegapixel;
        private KryptonContextMenuRadioButton eightMegapixel;
        private KryptonContextMenuRadioButton tenMegapixel;
        private KryptonContextMenuRadioButton twelveMegapixel;
        private KryptonContextMenuRadioButton custom;

        private KryptonRibbonGroupColorButton backgroundColorButton;

        public RenderGUIController(BasicForm form, BasicController basicController, ShortcutController shortcuts)
        {
            this.form = form;
            this.imageRenderer = basicController.ImageRenderer;
            this.drawingWindowController = basicController.DrawingWindowController;
            backgroundColorButton = form.renderingBackgroundColor;

            width = form.renderWidthUpDown;
            height = form.renderHeightUpDown;

            form.renderCommand.Execute += new EventHandler(renderCommand_Execute);

            //Shortcuts
            ShortcutGroup group = shortcuts.createOrRetrieveGroup("RenderingShortcuts");

            ShortcutEventCommand render = new ShortcutEventCommand("Render", Keys.F11, false);
            render.Execute += new ShortcutEventCommand.ExecuteEvent(render_Execute);
            group.addShortcut(render);

            //Resolution menu
            resolutionMenu = new KryptonContextMenu();
            KryptonContextMenuHeading heading = new KryptonContextMenuHeading("Resolution");
            resolutionMenu.Items.Add(heading);

            onePointThreeMegapixel = new KryptonContextMenuRadioButton("1.3 Megapixel");
            onePointThreeMegapixel.CheckedChanged += new EventHandler(onePointThreeMegapixel_CheckedChanged);
            resolutionMenu.Items.Add(onePointThreeMegapixel);

            fourMegapixel = new KryptonContextMenuRadioButton("4 Megapixel");
            fourMegapixel.CheckedChanged += new EventHandler(fourMegapixel_CheckedChanged);
            resolutionMenu.Items.Add(fourMegapixel);

            sixMegapixel = new KryptonContextMenuRadioButton("6 Megapixel");
            sixMegapixel.CheckedChanged += new EventHandler(sixMegapixel_CheckedChanged);
            resolutionMenu.Items.Add(sixMegapixel);

            eightMegapixel = new KryptonContextMenuRadioButton("8 Megapixel");
            eightMegapixel.CheckedChanged += new EventHandler(eightMegapixel_CheckedChanged);
            resolutionMenu.Items.Add(eightMegapixel);

            tenMegapixel = new KryptonContextMenuRadioButton("10 Megapixel");
            tenMegapixel.CheckedChanged += new EventHandler(tenMegapixel_CheckedChanged);
            resolutionMenu.Items.Add(tenMegapixel);

            twelveMegapixel = new KryptonContextMenuRadioButton("12 Megapixel");
            twelveMegapixel.CheckedChanged += new EventHandler(twelveMegapixel_CheckedChanged);
            resolutionMenu.Items.Add(twelveMegapixel);

            custom = new KryptonContextMenuRadioButton("Custom");
            custom.Checked = true;
            custom.CheckedChanged += new EventHandler(custom_CheckedChanged);
            resolutionMenu.Items.Add(custom);

            form.renderImageSizeButton.KryptonContextMenu = resolutionMenu;
        }

        void renderCommand_Execute(object sender, EventArgs e)
        {
            DrawingWindowHost drawingWindow = drawingWindowController.getActiveWindow();
            if (drawingWindow != null)
            {
                int width = (int)this.width.Value;
                int height = (int)this.height.Value;
                ImageRendererProperties imageProperties = new ImageRendererProperties();
                imageProperties.Width = width;
                imageProperties.Height = height;
                imageProperties.UseWindowBackgroundColor = false;
                imageProperties.CustomBackgroundColor = Engine.Color.FromARGB(backgroundColorButton.SelectedColor.ToArgb());
                Bitmap bitmap = imageRenderer.renderImage(imageProperties);
                if (bitmap != null)
                {
                    KryptonPictureWindow picture = new KryptonPictureWindow();
                    picture.initialize(bitmap);
                    picture.Text = String.Format("{0} - {1}x{2}", drawingWindow.Text, width, height);
                    picture.show(form.DockingManager);
                }
            }
        }

        void render_Execute(ShortcutEventCommand shortcut)
        {
            renderCommand_Execute(null, null);
        }

        void custom_CheckedChanged(object sender, EventArgs e)
        {
            width.Enabled = custom.Checked;
            height.Enabled = custom.Checked;
        }

        void twelveMegapixel_CheckedChanged(object sender, EventArgs e)
        {
            if (twelveMegapixel.Checked)
            {
                width.Value = 4000;
                height.Value = 3000;
            }
        }

        void tenMegapixel_CheckedChanged(object sender, EventArgs e)
        {
            if (tenMegapixel.Checked)
            {
                width.Value = 3648;
                height.Value = 2736;
            }
        }

        void eightMegapixel_CheckedChanged(object sender, EventArgs e)
        {
            if (eightMegapixel.Checked)
            {
                width.Value = 3456;
                height.Value = 2304;
            }
        }

        void sixMegapixel_CheckedChanged(object sender, EventArgs e)
        {
            if (sixMegapixel.Checked)
            {
                width.Value = 3000;
                height.Value = 2000;
            }
        }

        void fourMegapixel_CheckedChanged(object sender, EventArgs e)
        {
            if (fourMegapixel.Checked)
            {
                width.Value = 2448;
                height.Value = 1632;
            }
        }

        void onePointThreeMegapixel_CheckedChanged(object sender, EventArgs e)
        {
            if (onePointThreeMegapixel.Checked)
            {
                width.Value = 1280;
                height.Value = 1024;
            }
        }
    }
}
