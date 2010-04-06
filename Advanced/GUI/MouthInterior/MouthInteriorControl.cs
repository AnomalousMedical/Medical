using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace Medical.GUI
{
    public partial class MouthInteriorControl : GUIElement
    {
        bool allowUpdates = true;

        public MouthInteriorControl()
        {
            InitializeComponent();

            Type enumType = typeof(TongueMode);
            foreach (FieldInfo fieldInfo in enumType.GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                tongueModeCombo.Items.Add(Enum.Parse(enumType, fieldInfo.Name));
            }

            tongueModeCombo.SelectedIndexChanged += new EventHandler(tongueModeCombo_SelectedIndexChanged);
            tonguePosition.ValueChanged += new EventHandler(tonguePosition_ValueChanged);
            tongueCollisionCheckBox.CheckedChanged += new EventHandler(tongueCollisionCheckBox_CheckedChanged);
            lipCollisionCheck.CheckedChanged += new EventHandler(lipCollisionCheck_CheckedChanged);
            topLipsRigidCheckBox.CheckedChanged += new EventHandler(topLipsRigidCheckBox_CheckedChanged);
            bottomLipsRigidCheck.CheckedChanged += new EventHandler(bottomLipsRigidCheck_CheckedChanged);
        }

        protected override void sceneLoaded(Engine.ObjectManagement.SimScene scene)
        {
            base.sceneLoaded(scene);
            this.Enabled = TongueController.TongueDetected;
            if (Enabled)
            {
                allowUpdates = false;
                tongueModeCombo.SelectedItem = TongueController.TongueMode;
                tonguePosition.Value = (decimal)TongueController.TonguePosition;
                tongueCollisionCheckBox.Checked = TongueController.TongueCollisionEnabled;

                lipCollisionCheck.Checked = LipController.LipCollisionEnabled;
                topLipsRigidCheckBox.Checked = LipController.TopLipsRigid;
                bottomLipsRigidCheck.Checked = LipController.BottomLipsRigid;
                allowUpdates = true;
            }
        }

        void tongueModeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                TongueController.TongueMode = (TongueMode)tongueModeCombo.SelectedItem;
            }
        }

        void tonguePosition_ValueChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                TongueController.TonguePosition = (float)tonguePosition.Value;
            }
        }

        void tongueCollisionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                TongueController.TongueCollisionEnabled = tongueCollisionCheckBox.Checked;
            }
        }

        void lipCollisionCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                LipController.LipCollisionEnabled = lipCollisionCheck.Checked;
            }
        }

        void topLipsRigidCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                LipController.TopLipsRigid = topLipsRigidCheckBox.Checked;
            }
        }

        void bottomLipsRigidCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                LipController.BottomLipsRigid = bottomLipsRigidCheck.Checked;
            }
        }

        private void resetLipsButton_Click(object sender, EventArgs e)
        {
            LipController.setOriginalPosition();
        }
    }
}
