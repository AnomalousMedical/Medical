using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Engine.Saving.XMLSaver;
using System.Xml;

namespace Medical.GUI
{
    public partial class PresetLayerEditor : GUIElement
    {
        private LayerController layerController;
        private XmlSaver xmlSaver = new XmlSaver();

        public PresetLayerEditor()
        {
            InitializeComponent();
            layerList.SelectedValueChanged += new EventHandler(layerList_SelectedValueChanged);
        }

        public void initialize(LayerController layerController)
        {
            this.layerController = layerController;
            layerController.LayerStateSetChanged += layerController_LayerStateSetChanged;
        }

        void layerController_LayerStateSetChanged(LayerController controller)
        {
            layerList.Items.Clear();
            LayerStateSet stateSet = controller.CurrentLayers;
            this.Enabled = stateSet != null;
            if (Enabled)
            {
                foreach (String name in stateSet.LayerStateNames)
                {
                    layerList.Items.Add(name);
                }
            }
        }

        void layerList_SelectedValueChanged(object sender, EventArgs e)
        {
            if (layerList.SelectedItem != null)
            {
                String name = layerList.SelectedItem.ToString();
                LayerState state = layerController.CurrentLayers.getState(name);
                hiddenCheckBox.Checked = state.Hidden;
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            InputResult result = InputBox.GetInput("Enter Name", "Enter a name for the preset layer", this, validateName);
            if (result.ok)
            {
                LayerState state = new LayerState(result.text);
                state.captureState();
                state.Hidden = hiddenCheckBox.Checked;
                layerController.CurrentLayers.addState(state);
                layerList.Items.Add(result.text);
            }
        }

        bool validateName(String input, out String newPrompt)
        {
            if (layerController.CurrentLayers.hasState(input))
            {
                newPrompt = "That name is already in use. Please enter another.";
                return false;
            }
            newPrompt = "";
            return true;
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (layerList.SelectedItem != null)
            {
                String name = layerList.SelectedItem.ToString();
                layerController.CurrentLayers.removeState(name);
                layerList.Items.Remove(name);
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (layerList.SelectedItem != null)
            {
                String name = layerList.SelectedItem.ToString();
                LayerState state = layerController.CurrentLayers.getState(name);
                state.Hidden = hiddenCheckBox.Checked;
                state.captureState();
            }
        }

        private void previewButton_Click(object sender, EventArgs e)
        {
            if (layerList.SelectedItem != null)
            {
                String name = layerList.SelectedItem.ToString();
                layerController.applyLayerState(name);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if(saveFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    layerController.saveLayerStateSet(saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, String.Format("An error occured when saving the layer states.\n{0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void loadButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    layerController.loadLayerStateSet(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, String.Format("An error occured when saving the layer states.\n{0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
