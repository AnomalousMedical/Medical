﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using System.IO;
using Engine.Saving.XMLSaver;
using System.Xml;
using Medical;

namespace Developer.GUI
{
    class DeveloperResolutionMenu : PopupContainer
    {
        static String RenderPresetsFile = Path.Combine(MedicalConfig.UserDocRoot, "DeveloperRenderPresets.ini");
        static XmlSaver xmlSaver = new XmlSaver();

        public event EventHandler ResolutionChanged;

        private MultiListBox presets;
        private DeveloperRenderPropertiesDialog renderDialog;

        public DeveloperResolutionMenu(DeveloperRenderPropertiesDialog renderDialog)
            : base("Developer.GUI.DeveloperRenderer.DeveloperResolutionMenu.layout")
        {
            this.renderDialog = renderDialog;

            Button addButton = (Button)widget.findWidget("AddButton");
            addButton.MouseButtonClick += new MyGUIEvent(addButton_MouseButtonClick);

            Button removeButton = (Button)widget.findWidget("RemoveButton");
            removeButton.MouseButtonClick += new MyGUIEvent(removeButton_MouseButtonClick);

            presets = (MultiListBox)widget.findWidget("PresetList");
            presets.addColumn("Preset", 50);
            presets.setColumnResizingPolicyAt(0, ResizingPolicy.Fill);
            if (!File.Exists(RenderPresetsFile))
            {
                presets.addItem("Web", new RenderPreset("Web", 640, 480));
                presets.addItem("Presentation", new RenderPreset("Presentation", 1024, 768));
            }
            else
            {
                try
                {
                    using (XmlTextReader xmlReader = new XmlTextReader(RenderPresetsFile))
                    {
                        RenderPresetCollection container = (RenderPresetCollection)xmlSaver.restoreObject(xmlReader);
                        foreach (RenderPreset preset in container)
                        {
                            presets.addItem(preset.Name, preset);
                        }
                    }
                }
                catch (Exception)
                {
                    try
                    {
                        File.Delete(RenderPresetsFile);
                    }
                    catch (Exception) { }
                    presets.addItem("Web", new RenderPreset("Web", 640, 480));
                    presets.addItem("Presentation", new RenderPreset("Presentation", 1024, 768));
                }
            }
            presets.ListChangePosition += new MyGUIEvent(presets_ListChangePosition);
        }

        public override void Dispose()
        {
            savePresets();
            base.Dispose();
        }

        void presets_ListChangePosition(Widget source, EventArgs e)
        {
            uint selectedIndex = presets.getIndexSelected();
            if (selectedIndex != uint.MaxValue)
            {
                RenderPreset preset = (RenderPreset)presets.getItemDataAt(selectedIndex);
                ImageWidth = preset.Width;
                ImageHeight = preset.Height;
                if (ResolutionChanged != null)
                {
                    ResolutionChanged.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private void savePresets()
        {
            try
            {
                RenderPresetCollection container = new RenderPresetCollection();
                uint count = presets.getItemCount();
                for (uint i = 0; i < count; ++i)
                {
                    container.AddLast((RenderPreset)presets.getItemDataAt(i));
                }
                using (XmlTextWriter xmlWriter = new XmlTextWriter(RenderPresetsFile, Encoding.Unicode))
                {
                    xmlWriter.Formatting = Formatting.Indented;
                    xmlSaver.saveObject(container, xmlWriter);
                }
            }
            catch (Exception ex)
            {
                Logging.Log.Error("{0} saving presets. Reason: {1}", ex.GetType().Name, ex.Message);
            }
        }
        
        void removeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            uint selectedIndex = presets.getIndexSelected();
            if (selectedIndex != uint.MaxValue)
            {
                presets.removeItemAt(selectedIndex);
            }
        }

        void addButton_MouseButtonClick(Widget source, EventArgs e)
        {
            InputBox.GetInput("Add Preset", "Enter a name for this preset.", true, delegate(String result, ref string errorPrompt)
            {
                presets.addItem(result, new RenderPreset(result, renderDialog.RenderWidth, renderDialog.RenderHeight));
                return true;
            });
        }

        public int ImageWidth { get; set; }

        public int ImageHeight { get; set; }
    }
}
