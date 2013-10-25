using Engine;
using Engine.Editing;
using libRocketPlugin;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    public class EditInterfaceEditor : ElementEditorComponent
    {
        private ScrollView propertiesScroll;
        private ScrollingExpandingEditInterfaceViewer propertiesForm;

        public EditInterfaceEditor(String title, EditInterface editInterface, MedicalUICallback uiCallback, RmlWysiwygBrowserProvider browserProvider)
            : base("Medical.GUI.Editor.RmlWysiwyg.ElementEditorComponents.EditInterfaceEditor.layout", title)
        {
            propertiesScroll = (ScrollView)widget.findWidget("PropertiesScroll");
            propertiesForm = new ScrollingExpandingEditInterfaceViewer(propertiesScroll, uiCallback);
            propertiesForm.EditInterface = editInterface;
        }

        public override void Dispose()
        {
            propertiesForm.Dispose();
            base.Dispose();
        }

        public void alertChangesMade()
        {
            this.fireChangesMade();
            this.fireApplyChanges();
        }

        public EditInterface EditInterface
        {
            get
            {
                return propertiesForm.EditInterface;
            }
            set
            {
                propertiesForm.EditInterface = value;
                propertiesForm.layout();
            }
        }

        public override void attachToParent(RmlElementEditor parentEditor, Widget parent)
        {
            base.attachToParent(parentEditor, parent);
            propertiesForm.layout();
        }
    }
}
