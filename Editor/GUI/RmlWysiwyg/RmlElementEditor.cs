using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using libRocketPlugin;
using Medical.Editor;

namespace Medical.GUI
{
    class RmlElementEditor : PopupContainer
    {
        /// <summary>
        /// Open a text editor that disposes when it is closed.
        /// </summary>
        /// <returns></returns>
        public static RmlElementEditor openTextEditor(Element element, int left, int top)
        {
            RmlElementEditor editor = new RmlElementEditor(element);
            editor.show(left, top);
            editor.Hidden += (source, e) =>
            {
                ((RmlElementEditor)source).Dispose();
            };
            return editor;
        }

        public event Action<Element> MoveElementUp;
        public event Action<Element> MoveElementDown;
        public event Action<Element> DeleteElement;

        private Element element;
        private TabControl tabs;

        protected RmlElementEditor(Element element)
            :base("Medical.GUI.RmlWysiwyg.RmlElementEditor.layout")
        {
            this.element = element;

            tabs = (TabControl)widget.findWidget("Tabs");

            Button applyButton = (Button)widget.findWidget("ApplyButton");
            applyButton.MouseButtonClick += new MyGUIEvent(applyButton_MouseButtonClick);

            Button cancelButton = (Button)widget.findWidget("CancelButton");
            cancelButton.MouseButtonClick += new MyGUIEvent(cancelButton_MouseButtonClick);

            Button up = (Button)widget.findWidget("Up");
            up.MouseButtonClick += new MyGUIEvent(up_MouseButtonClick);

            Button down = (Button)widget.findWidget("Down");
            down.MouseButtonClick += new MyGUIEvent(down_MouseButtonClick);

            Button delete = (Button)widget.findWidget("Delete");
            delete.MouseButtonClick += new MyGUIEvent(delete_MouseButtonClick);

            ApplyChanges = true;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public void addElementEditor(ElementEditorComponent editorComponent)
        {
            TabItem tab = tabs.addItem(editorComponent.Name);
            editorComponent.attachToParent(this, tab);
        }

        public String UndoRml { get; set; }

        public bool ApplyChanges { get; set; }

        void applyButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.hide();
        }

        void cancelButton_MouseButtonClick(Widget source, EventArgs e)
        {
            ApplyChanges = false;
            this.hide();
        }

        void delete_MouseButtonClick(Widget source, EventArgs e)
        {
            if (DeleteElement != null)
            {
                DeleteElement.Invoke(element);
            }
            hide();
        }

        void down_MouseButtonClick(Widget source, EventArgs e)
        {
            if (MoveElementDown != null)
            {
                MoveElementDown.Invoke(element);
            }
        }

        void up_MouseButtonClick(Widget source, EventArgs e)
        {
            if (MoveElementUp != null)
            {
                MoveElementUp.Invoke(element);
            }
        }
    }
}
