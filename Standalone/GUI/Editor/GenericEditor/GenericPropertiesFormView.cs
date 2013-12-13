using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI.AnomalousMvc;
using Engine.Editing;
using Engine.Saving;
using Engine.Attributes;

namespace Medical.GUI
{
    public class GenericPropertiesFormView : MyGUIView
    {
        public GenericPropertiesFormView(String name, EditInterface editInterface, EditorController editorController, MedicalUICallback editUICallback, bool horizontalAlignment = false)
            : base(name)
        {
            this.EditInterface = editInterface;
            this.HorizontalAlignment = horizontalAlignment;
            this.EditorController = editorController;
            this.EditUICallback = editUICallback;
        }

        public EditInterface EditInterface { get; set; }

        public bool HorizontalAlignment { get; set; }

        public MedicalUICallback EditUICallback { get; set; }

        public EditorController EditorController { get; set; }

        protected GenericPropertiesFormView(LoadInfo info)
            : base(info)
        {

        }
    }
}
