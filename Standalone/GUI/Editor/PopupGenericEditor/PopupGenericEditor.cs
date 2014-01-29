using Engine.Editing;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.GUI
{
    public class PopupGenericEditor : PopupContainer
    {
        private ScrollView propertiesScroll;
        private ScrollablePropertiesForm propertiesForm;

        /// <summary>
        /// Open a text editor that disposes when it is closed.
        /// </summary>
        /// <returns></returns>
        public static PopupGenericEditor openEditor(EditInterface editInterface, MedicalUICallback uiCallback, int left, int top)
        {
            PopupGenericEditor editor = new PopupGenericEditor(editInterface, uiCallback);
            editor.show(left, top);
            editor.Hidden += (source, e) =>
            {
                ((PopupGenericEditor)source).Dispose();
            };
            return editor;
        }

        public PopupGenericEditor(EditInterface editInterface, MedicalUICallback uiCallback)
            :base("Medical.GUI.Editor.PopupGenericEditor.PopupGenericEditor.layout")
        {
            propertiesScroll = (ScrollView)widget.findWidget("PropertiesScroll");
            propertiesForm = new ScrollablePropertiesForm(propertiesScroll, uiCallback);
            propertiesForm.EditInterface = editInterface;

            Button close = (Button)widget.findWidget("Close");
            close.MouseButtonClick += close_MouseButtonClick;
        }

        void close_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
        }
    }
}
