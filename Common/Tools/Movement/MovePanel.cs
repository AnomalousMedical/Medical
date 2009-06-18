using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.ObjectManagement;
using Engine;
using WeifenLuo.WinFormsUI.Docking;

namespace Medical
{
    public partial class MovePanel : DockContent
    {
        MoveController moveController;
        private bool allowMotionUpdates = true;
        private bool allowObjectUpdate = true;

        public MovePanel()
        {
            InitializeComponent();

            xLoc.Maximum = decimal.MaxValue;
            xLoc.Minimum = decimal.MinValue;
            yLoc.Maximum = decimal.MaxValue;
            yLoc.Minimum = decimal.MinValue;
            zLoc.Maximum = decimal.MaxValue;
            zLoc.Minimum = decimal.MinValue;

            xLoc.ValueChanged += new EventHandler(moveObject);
            yLoc.ValueChanged += new EventHandler(moveObject);
            zLoc.ValueChanged += new EventHandler(moveObject);
            xLoc.KeyPress += new KeyPressEventHandler(xLoc_KeyPress);
            yLoc.KeyPress += new KeyPressEventHandler(yLoc_KeyPress);
            zLoc.KeyPress += new KeyPressEventHandler(zLoc_KeyPress);
        }

        public void initialize(MoveController moveController)
        {
            this.moveController = moveController;
            moveController.OnTranslationChanged += new TranslationChanged(selectionManager_OnTranslationChanged);
        }

        void selectionManager_OnTranslationChanged(Vector3 newTranslation, object sender)
        {
            if (allowMotionUpdates && this != sender)
            {
                allowObjectUpdate = false;
                xLoc.Value = (decimal)newTranslation.x;
                yLoc.Value = (decimal)newTranslation.y;
                zLoc.Value = (decimal)newTranslation.z;
                allowObjectUpdate = true;
            }
        }

        void zLoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' || e.KeyChar == '\t')
            {
                xLoc.Focus();
                xLoc.Select(0, xLoc.Text.Length);
            }
        }

        void yLoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r' || e.KeyChar == '\t')
            {
                zLoc.Focus();
                zLoc.Select(0, zLoc.Text.Length);
            }
        }

        void xLoc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == '\r' ||  e.KeyChar == '\t')
            {
                yLoc.Focus();
                yLoc.Select(0, yLoc.Text.Length);
            }
        }

        void moveObject(object sender, EventArgs e)
        {
            if (allowObjectUpdate)
            {
                allowMotionUpdates = false;
                Vector3 newPos = new Vector3((float)xLoc.Value, (float)yLoc.Value, (float)zLoc.Value);
                moveController.setTranslation(ref newPos, this);
                allowMotionUpdates = true;
            }
        }
    }
}
