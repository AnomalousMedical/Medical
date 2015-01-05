using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Editing;
using Engine.Saving;
using Engine.Attributes;
using Anomalous.GuiFramework.Editor;

namespace Medical.GUI
{
    public class ExpandingGenericEditorView : MyGUIView
    {
        public ExpandingGenericEditorView(String name, EditInterface editInterface, EditorController editorController, GuiFrameworkUICallback editUICallback, bool horizontalAlignment = false)
            : base(name)
        {
            this.EditInterface = editInterface;
            this.HorizontalAlignment = horizontalAlignment;
            this.EditorController = editorController;
            this.EditUICallback = editUICallback;
        }

        public EditInterface EditInterface { get; set; }

        public bool HorizontalAlignment { get; set; }

        public GuiFrameworkUICallback EditUICallback { get; set; }

        public EditorController EditorController { get; set; }

        protected ExpandingGenericEditorView(LoadInfo info)
            : base(info)
        {

        }
    }
}
