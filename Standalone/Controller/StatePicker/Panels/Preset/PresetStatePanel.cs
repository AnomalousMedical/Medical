using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using System.Xml;
using Logging;

namespace Medical.GUI
{
    class PresetStatePanel : StateWizardPanel
    {
        private ButtonGridItem defaultItem = null;
        private ButtonGridItem openingItem = null; //the item that was selected when this ui was opened.
        private bool allowUpdates = true;
        //private LinkedList<Image> images = new LinkedList<Image>();
        private String lastRootDirectory;
        private String subDirectory;

        private ButtonGrid presetListView;

        public PresetStatePanel(String subDirectory, String presetPanelFile, StateWizardPanelController controller)
            : base(presetPanelFile, controller)
        {
            this.subDirectory = subDirectory;

            presetListView = new ButtonGrid(mainWidget.findWidget("PresetPanel/ScrollView") as ScrollView);
            presetListView.SelectedValueChanged += new EventHandler(presetListView_SelectedValueChanged);
        }

        void presetListView_SelectedValueChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                showChanges(false);
            }
        }

        public override void sceneChanged(MedicalController medicalController, SimulationScene simScene)
        {
            String rootDirectory = medicalController.CurrentSceneDirectory + '/' + simScene.PresetDirectory;
            if (rootDirectory != lastRootDirectory)
            {
                lastRootDirectory = rootDirectory;

                PresetStateSet presets = new PresetStateSet(subDirectory, rootDirectory + '/' + subDirectory);
                loadPresetSet(presets);
                this.clear();
                this.initialize(presets);
            }
        }

        public void initialize(PresetStateSet presetStateSet)
        {
            //if (presetListView.LargeImageList == null)
            //{
            //    presetListView.LargeImageList = panelController.ImageList;
            //}
            VirtualFileSystem archive = VirtualFileSystem.Instance;
            presetListView.SuppressLayout = true;
            foreach (PresetState state in presetStateSet.Presets)
            {
                //String fullImageName = presetStateSet.SourceDirectory + "/" + state.ImageName;
                //if (!presetListView.LargeImageList.Images.ContainsKey(fullImageName))
                //{
                //    try
                //    {
                //        using (Stream imageStream = archive.openStream(fullImageName, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read))
                //        {
                //            Image image = Image.FromStream(imageStream);
                //            if (image != null)
                //            {
                //                presetListView.LargeImageList.Images.Add(fullImageName, image);
                //                images.AddLast(image);
                //                image.Tag = fullImageName;
                //            }
                //        }
                //    }
                //    catch (Exception e)
                //    {
                //        Log.Error("Could not open image preview file {0}.", fullImageName);
                //    }
                //}
                ButtonGridItem item = presetListView.addItem(state.Category, state.Name);// new ListViewItem(state.Name, fullImageName);
                item.UserObject = state;
            }
            presetListView.SuppressLayout = false;
            presetListView.layout();
            if (presetListView.Count > 0)
            {
                defaultItem = presetListView.getItem(0);
            }
        }

        public void clear()
        {
            clearImages();
            presetListView.clear();
        }

        public override void applyToState(MedicalState state)
        {
            if (presetListView.SelectedItem != null)
            {
                PresetState preset = presetListView.SelectedItem.UserObject as PresetState;
                preset.applyToState(state);
            }
            else
            {
                PresetState preset = defaultItem.UserObject as PresetState;
                preset.applyToState(state);
            }
        }

        public override void setToDefault()
        {
            allowUpdates = false;
            presetListView.SelectedItem = defaultItem;
            allowUpdates = true;
        }

        public override void recordOpeningState()
        {
            openingItem = presetListView.SelectedItem;
        }

        public override void resetToOpeningState()
        {
            allowUpdates = false;
            presetListView.SelectedItem = openingItem;
            allowUpdates = true;
        }

        private void clearImages()
        {
            //foreach (Image image in images)
            //{
            //    panelController.ImageList.Images.RemoveByKey(image.Tag.ToString());
            //    image.Dispose();
            //}
            //images.Clear();
        }

        private void loadPresetSet(PresetStateSet presetStateSet)
        {
            VirtualFileSystem archive = VirtualFileSystem.Instance;
            String[] files = archive.listFiles(presetStateSet.SourceDirectory, "*.pre", false);
            foreach (String file in files)
            {
                XmlTextReader reader = new XmlTextReader(archive.openStream(file, Engine.Resources.FileMode.Open, Engine.Resources.FileAccess.Read));
                PresetState preset = controller.XmlSaver.restoreObject(reader) as PresetState;
                if (preset != null)
                {
                    presetStateSet.addPresetState(preset);
                }
                else
                {
                    Log.Error("Could not load preset from file {0}. Object was not a BoneManipulatorPresetState.", file);
                }
                reader.Close();
            }
        }
    }
}
