using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using System.Xml;
using Logging;
using System.IO;
using Medical.Controller.AnomalousMvc;
using Engine.Saving.XMLSaver;
using FreeImageAPI;

namespace Medical.GUI.AnomalousMvc
{
    class PresetStateGUI : WizardComponent<PresetStateView>
    {
        private static XmlSaver xmlSaver = new XmlSaver();
        private ButtonGridItem defaultItem = null;
        private bool allowUpdates = true;
        private String subDirectory;
        private ImageAtlas imageAtlas;

        private SingleSelectButtonGrid presetListView;

        public PresetStateGUI(PresetStateView wizardView, AnomalousMvcContext context, MyGUIViewHost viewHost)
            : base("Medical.GUI.DistortionWizard.Preset.PresetStateGUI.layout", wizardView, context, viewHost)
        {
            this.subDirectory = wizardView.PresetDirectory;

            presetListView = new SingleSelectButtonGrid(widget as ScrollView);
            presetListView.SelectedValueChanged += presetListView_SelectedValueChanged;

            imageAtlas = new ImageAtlas("PresetStateGUI_" + subDirectory, new IntSize2(100, 100));
        }

        public override void Dispose()
        {
            presetListView.SelectedValueChanged -= presetListView_SelectedValueChanged;
            presetListView.Dispose();
            presetListView = null;
            imageAtlas.Dispose();
            base.Dispose();
        }

        public override void topLevelResized()
        {
            base.topLevelResized();
            presetListView.resizeAndLayout();
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
            Coroutine.Start(initialize());
        }

        public IEnumerator<YieldAction> initialize()
        {
            this.clear();
            yield return Coroutine.WaitSeconds(0.1f);

            foreach (PresetState state in loadPresets(subDirectory))
            {
                if(presetListView == null)
                {
                    yield break;
                }

                String fullImageName = subDirectory + "/" + state.ImageName;
                String imageKey = null;
                if (!imageAtlas.containsImage(fullImageName))
                {
                    try
                    {
                        using (Stream imageStream = context.ResourceProvider.openFile(fullImageName))
                        {
                            using (var image = new FreeImageBitmap(imageStream))
                            {
                                if (image != null)
                                {
                                    imageKey = imageAtlas.addImage(fullImageName, image);
                                    image.Tag = fullImageName;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        Log.Error("Could not open image preview file {0}.", fullImageName);
                    }
                }
                presetListView.SuppressLayout = true;

                ButtonGridItem item = presetListView.addItem(state.Category, state.Name, imageKey);
                item.UserObject = state;

                presetListView.SuppressLayout = false;
                presetListView.layout();

                if (presetListView.Count == 1) //Select first item
                {
                    defaultItem = presetListView.getItem(0);
                }

                yield return Coroutine.WaitSeconds(0.1f);
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

        private IEnumerable<PresetState> loadPresets(String sourceDirectory)
        {
            IEnumerable<String> files = context.ResourceProvider.listFiles("*.pre", sourceDirectory, false);
            foreach (String file in files)
            {
                using (XmlTextReader reader = new XmlTextReader(context.ResourceProvider.openFile(file)))
                {
                    PresetState preset = xmlSaver.restoreObject(reader) as PresetState;
                    if (preset != null)
                    {
                        yield return preset;
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
