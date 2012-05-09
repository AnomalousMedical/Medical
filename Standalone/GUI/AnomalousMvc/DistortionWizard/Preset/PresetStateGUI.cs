using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using System.Xml;
using Logging;
using System.IO;
using System.Drawing;
using Medical.Controller.AnomalousMvc;
using Engine.Saving.XMLSaver;

namespace Medical.GUI.AnomalousMvc
{
    class PresetStateGUI : WizardPanel<PresetStateView>
    {
        private static XmlSaver xmlSaver = new XmlSaver();
        private ButtonGridItem defaultItem = null;
        private bool allowUpdates = true;
        private String subDirectory;
        private ImageAtlas imageAtlas;

        private ButtonGrid presetListView;

        public PresetStateGUI(PresetStateView wizardView, AnomalousMvcContext context)
            : base("Medical.GUI.DistortionWizards.Preset.PresetStateGUI.layout", wizardView, context)
        {
            this.subDirectory = wizardView.PresetDirectory;

            presetListView = new ButtonGrid(widget.findWidget("PresetPanel/ScrollView") as ScrollView);
            presetListView.SelectedValueChanged += new EventHandler(presetListView_SelectedValueChanged);

            imageAtlas = new ImageAtlas("PresetStateGUI_" + subDirectory, new Size2(100, 100), new Size2(512, 512));
        }

        public override void Dispose()
        {
            imageAtlas.Dispose();
            base.Dispose();
        }

        void presetListView_SelectedValueChanged(object sender, EventArgs e)
        {
            if (allowUpdates)
            {
                PresetState preset = defaultItem.UserObject as PresetState;
                if (presetListView.SelectedItem != null)
                {
                    preset = presetListView.SelectedItem.UserObject as PresetState;
                }
                context.applyPresetState(preset, 1.0f);
            }
        }

        public override void opening()
        {
            PresetStateSet presets = new PresetStateSet(subDirectory, subDirectory);
            loadPresetSet(presets);
            this.clear();
            this.initialize(presets);
        }

        public void initialize(PresetStateSet presetStateSet)
        {
            presetListView.SuppressLayout = true;
            foreach (PresetState state in presetStateSet.Presets)
            {
                String fullImageName = presetStateSet.SourceDirectory + "/" + state.ImageName;
                String imageKey = null;
                if (!imageAtlas.containsImage(fullImageName))
                {
                    try
                    {
                        using (Stream imageStream = context.ResourceProvider.openFile(fullImageName))
                        {
                            Image image = Image.FromStream(imageStream);
                            if (image != null)
                            {
                                imageKey = imageAtlas.addImage(fullImageName, image);
                                image.Tag = fullImageName;
                                image.Dispose();
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Log.Error("Could not open image preview file {0}.", fullImageName);
                    }
                }
                ButtonGridItem item = presetListView.addItem(state.Category, state.Name, imageKey);
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

        private void clearImages()
        {
            imageAtlas.clear();
        }

        private void loadPresetSet(PresetStateSet presetStateSet)
        {
            String[] files = context.ResourceProvider.listFiles("*.pre", presetStateSet.SourceDirectory, false);
            foreach (String file in files)
            {
                using (XmlTextReader reader = new XmlTextReader(context.ResourceProvider.openFile(file)))
                {
                    PresetState preset = xmlSaver.restoreObject(reader) as PresetState;
                    if (preset != null)
                    {
                        presetStateSet.addPresetState(preset);
                    }
                    else
                    {
                        Log.Error("Could not load preset from file {0}. Object was not a BoneManipulatorPresetState.", file);
                    }
                }
            }
        }
    }
}
