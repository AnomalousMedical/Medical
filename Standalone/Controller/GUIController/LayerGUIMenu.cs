using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CEGUIPlugin;

namespace Medical.GUI
{
    public delegate void ChangeTransparency(float value);

    class LayerGUIMenu : IDisposable
    {
        public event ChangeTransparency TransparencyChanged;

        protected PushButton mainButton;
        //protected KryptonContextMenu contextMenu;
        //protected KryptonContextMenuRadioButton opaqueButton;
        //protected KryptonContextMenuRadioButton transparentButton;
        //protected KryptonContextMenuRadioButton hiddenButton;

        private bool allowUpdates = true;
        private float currentTransparency = 1.0f;

        public LayerGUIMenu(PushButton mainButton)
        {
            //contextMenu = new KryptonContextMenu();
            
            //KryptonContextMenuHeading visibilityHeading = new KryptonContextMenuHeading("Visibility");
            //contextMenu.Items.Add(visibilityHeading);
            
            //opaqueButton = new KryptonContextMenuRadioButton("Opaque");
            //opaqueButton.Checked = true;
            //opaqueButton.AutoClose = true;
            //opaqueButton.CheckedChanged += new EventHandler(opaqueButton_CheckedChanged);
            //contextMenu.Items.Add(opaqueButton);

            //transparentButton = new KryptonContextMenuRadioButton("Transparent");
            //transparentButton.AutoClose = true;
            //transparentButton.CheckedChanged += new EventHandler(transparentButton_CheckedChanged);
            //contextMenu.Items.Add(transparentButton);

            //hiddenButton = new KryptonContextMenuRadioButton("Hidden");
            //hiddenButton.AutoClose = true;
            //hiddenButton.CheckedChanged += new EventHandler(hiddenButton_CheckedChanged);
            //contextMenu.Items.Add(hiddenButton);

            this.mainButton = mainButton;
            mainButton.Clicked += new CEGUIEvent(mainButton_Click);
            //mainButton.KryptonContextMenu = contextMenu;
        }

        //public virtual void createShortcuts(String name, ShortcutGroup shortcutGroup, Keys key)
        //{
        //    ShortcutEventCommand shortcut = new ShortcutEventCommand(name, key, false);
        //    shortcut.Execute += new ShortcutEventCommand.ExecuteEvent(shortcut_Execute);
        //    shortcutGroup.addShortcut(shortcut);
        //}

        public void Dispose()
        {
            //contextMenu.Dispose();
        }

        public void setAlpha(float alpha)
        {
            allowUpdates = false;
            if (alpha >= 1.0f)
            {
                //opaqueButton.Checked = true;
            }
            else if (alpha <= 0.0f)
            {
                //hiddenButton.Checked = true;
            }
            else
            {
                //transparentButton.Checked = true;
            }
            allowUpdates = true;
        }

        //void shortcut_Execute(ShortcutEventCommand shortcut)
        //{
        //    mainButton_Click(null, null);
        //}

        void mainButton_Click(EventArgs e)
        {
            //if (opaqueButton.Checked)
            //{
            //    transparentButton.Checked = true;
            //}
            //else if (transparentButton.Checked)
            //{
            //    hiddenButton.Checked = true;
            //}
            //else if (hiddenButton.Checked)
            //{
            //    opaqueButton.Checked = true;
            //}
            //temp transparency change algo
            if(currentTransparency == 1.0f)
            {
                currentTransparency = 0.7f;
            }
            else if(currentTransparency == 0.7f)
            {
                currentTransparency = 0.0f;
            }
            else if(currentTransparency == 0.0f)
            {
                currentTransparency = 1.0f;
            }
            
            if(TransparencyChanged != null)
            {
                TransparencyChanged(currentTransparency);
            }
        }

        //void opaqueButton_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (allowUpdates && opaqueButton.Checked && TransparencyChanged != null)
        //    {
        //        TransparencyChanged(1.0f);
        //    }
        //}

        //void transparentButton_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (allowUpdates && transparentButton.Checked && TransparencyChanged != null)
        //    {
        //        TransparencyChanged(0.7f);
        //    }
        //}

        //void hiddenButton_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (allowUpdates && hiddenButton.Checked && TransparencyChanged != null)
        //    {
        //        TransparencyChanged(0.0f);
        //    }
        //}
    }
}
