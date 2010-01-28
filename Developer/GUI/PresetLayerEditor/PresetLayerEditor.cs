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
using DragNDrop;

namespace Medical.GUI
{
    public partial class PresetLayerEditor : GUIElement
    {
        private static Engine.Color BACK_COLOR = new Engine.Color(.94f, .94f, .94f);

        private LayerController layerController;
        private XmlSaver xmlSaver = new XmlSaver();
        private ImageRenderer imageRenderer;

        public PresetLayerEditor()
        {
            InitializeComponent();
            layerList.SelectedIndexChanged += new EventHandler(layerList_SelectedValueChanged);
            this.SizeChanged += new EventHandler(PresetLayerEditor_SizeChanged);
            layerList.DragDrop += new DragEventHandler(layerList_DragDrop);
        }

        void PresetLayerEditor_SizeChanged(object sender, EventArgs e)
        {
            layerList.Columns[0].Width = -2;
        }

        public void initialize(LayerController layerController, ImageRenderer imageRenderer)
        {
            this.layerController = layerController;
            this.imageRenderer = imageRenderer;
            layerController.LayerStateSetChanged += layerController_LayerStateSetChanged;
        }

        void layerController_LayerStateSetChanged(LayerController controller)
        {
            layerList.Items.Clear();
            LayerStateSet stateSet = controller.CurrentLayers;
            this.Enabled = stateSet != null;
            if (Enabled)
            {
                foreach (LayerState state in stateSet.LayerStates)
                {
                    ListViewItem lvItem = new ListViewItem(state.Name);
                    lvItem.Tag = state;
                    layerList.Items.Add(lvItem);
                }
            }
        }

        void layerList_SelectedValueChanged(object sender, EventArgs e)
        {
            if (layerList.SelectedItems.Count > 0 && layerList.SelectedItems[0] != null)
            {
                String name = layerList.SelectedItems[0].Text;
                LayerState state = layerController.CurrentLayers.getState(name);
                hiddenCheckBox.Checked = state.Hidden;
                if (state.Thumbnail != null)
                {
                    Bitmap clone = new Bitmap(state.Thumbnail);
                    ThumbnailImage = clone;
                }
                else
                {
                    ThumbnailImage = null;
                }

                layerController.applyLayerState(name);
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
                ListViewItem lvItem = new ListViewItem(state.Name);
                lvItem.Tag = state;
                layerList.Items.Add(lvItem);
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
            if (layerList.SelectedItems.Count > 0 && layerList.SelectedItems[0] != null)
            {
                String name = layerList.SelectedItems[0].Text;
                layerController.CurrentLayers.removeState(name);
                layerList.Items.Remove(layerList.SelectedItems[0]);
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            if (layerList.SelectedItems.Count > 0 && layerList.SelectedItems[0] != null)
            {
                String name = layerList.SelectedItems[0].Text;
                LayerState state = layerController.CurrentLayers.getState(name);
                state.Hidden = hiddenCheckBox.Checked;
                state.captureState();
                if (ThumbnailImage != null)
                {
                    state.Thumbnail = new Bitmap(ThumbnailImage);
                }
                else
                {
                    ThumbnailImage = null;
                }
            }
        }

        void layerList_DragDrop(object sender, DragEventArgs e)
        {
            DragItemData data = (DragItemData)e.Data.GetData(typeof(DragItemData).ToString());
            foreach (ListViewItem item in data.DragItems)
            {
                layerController.CurrentLayers.moveState(item.Tag as LayerState, item.Index);
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

        private void renderThumbnailButton_Click(object sender, EventArgs e)
        {
            ImageRendererProperties properties = new ImageRendererProperties();
            properties.Width = thumbnailPanel.Width;
            properties.Height = thumbnailPanel.Height;
            properties.TransparentBackground = true;
            properties.UseWindowBackgroundColor = false;
            properties.CustomBackgroundColor = BACK_COLOR;
            properties.AntiAliasingMode = 8;

            Bitmap bitmap = imageRenderer.renderImage(properties);
            ThumbnailImage = bitmap;
        }

        private Bitmap ThumbnailImage
        {
            get
            {
                return thumbnailPanel.BackgroundImage as Bitmap;
            }
            set
            {
                if (thumbnailPanel.BackgroundImage != null)
                {
                    thumbnailPanel.BackgroundImage.Dispose();
                }
                thumbnailPanel.BackgroundImage = value;
            }
        }

        private void mergeButton_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            {
                try
                {
                    layerController.mergeLayerSet(openFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, String.Format("An error occured when saving the layer states.\n{0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
