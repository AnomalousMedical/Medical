using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using System.IO;

namespace Medical.GUI
{
    class ResolutionMenu : PopupContainer
    {
        public event EventHandler ResolutionChanged;

        private MultiList presets;

        public ResolutionMenu()
            : base("Medical.GUI.Render.ResolutionMenu.layout")
        {
            presets = (MultiList)widget.findWidget("PresetList");
            String renderPresetsFile = Path.Combine(MedicalConfig.UserDocRoot, "RenderPresets.ini");
            if (!File.Exists(renderPresetsFile))
            {
                presets.addColumn("Preset", presets.Width);
                presets.addItem("Web", new IntSize2(640, 480));
                presets.addItem("Presentation", new IntSize2(1024, 768));
            }
            else
            {

            }
            presets.ListChangePosition += new MyGUIEvent(presets_ListChangePosition);
        }

        void presets_ListChangePosition(Widget source, EventArgs e)
        {
            uint selectedIndex = presets.getIndexSelected();
            if (selectedIndex != uint.MaxValue)
            {
                IntSize2 size = (IntSize2)presets.getItemDataAt(selectedIndex);
                ImageWidth = size.Width;
                ImageHeight = size.Height;
                if (ResolutionChanged != null)
                {
                    ResolutionChanged.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public int ImageWidth { get; set; }

        public int ImageHeight { get; set; }
    }
}
