using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Saving;
using Engine.Editing;

namespace Medical.GUI
{
    public class GenericEditorView : MyGUIView
    {
        public GenericEditorView(String name, EditInterface editInterface, MedicalUICallback editUICallback, EditorController editorController, bool horizontalAlignment = false)
            : base(name)
        {
            this.EditInterface = editInterface;
            this.EditUICallback = editUICallback;
            this.EditorController = editorController;
            this.HorizontalAlignment = horizontalAlignment;
        }

        public EditInterface EditInterface { get; set; }

        public MedicalUICallback EditUICallback { get; set; }

        public EditorController EditorController { get; set; }

        public bool HorizontalAlignment { get; set; }

        protected GenericEditorView(LoadInfo info)
            :base(info)
        {

        }
    }
}
