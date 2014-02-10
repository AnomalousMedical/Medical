using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine.Editing;
using libRocketPlugin;
using Medical.Editor;
using Engine;

namespace Medical.GUI
{
    public class RmlElementEditor : PopupContainer
    {
        /// <summary>
        /// This delegate will be used when changes should be applied to the document / element from the editor.
        /// Return true if changes were made that need to be updated / recorded.
        /// </summary>
        /// <param name="element">The element that will be changed.</param>
        /// <param name="editor">The editor that edited the element.</param>
        /// <param name="component">The Wysiwyg editor component should be updated.</param>
        /// <returns>Return true if changes were made that need to be updated / recorded.</returns>
        public delegate bool ModifyDocumentDelegate(Element element, RmlElementEditor editor, RmlWysiwygComponent component);

        /// <summary>
        /// Open a text editor that disposes when it is closed.
        /// </summary>
        /// <returns></returns>
        public static RmlElementEditor openEditor(Element element, int left, int top, ElementStrategy elementStrategy)
        {
            RmlElementEditor editor = new RmlElementEditor(element, elementStrategy);
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
        public event Action<Element> ChangesMade;

        private ElementStrategy elementStrategy;

        private bool hasChanges;

        private Element element;
        private TabControl tabs;
        private List<ElementEditorComponent> editorComponents = new List<ElementEditorComponent>();
        private bool checkDeleteOnClose = true;

        protected RmlElementEditor(Element element, ElementStrategy elementStrategy)
            : base("Medical.GUI.Editor.RmlWysiwyg.RmlElementEditor.layout")
        {
            this.element = element;
            this.elementStrategy = elementStrategy;
            this.Hiding += RmlElementEditor_Hiding;

            tabs = (TabControl)widget.findWidget("Tabs");

            Button up = (Button)widget.findWidget("Up");
            up.MouseButtonClick += new MyGUIEvent(up_MouseButtonClick);

            Button down = (Button)widget.findWidget("Down");
            down.MouseButtonClick += new MyGUIEvent(down_MouseButtonClick);

            Button delete = (Button)widget.findWidget("Delete");
            delete.MouseButtonClick += new MyGUIEvent(delete_MouseButtonClick);

            Button close = (Button)widget.findWidget("Close");
            close.MouseButtonClick += close_MouseButtonClick;

            hasChanges = false;
            SmoothShow = false;
            KeepOpen = true;
        }

        public override void Dispose()
        {
            foreach (ElementEditorComponent component in editorComponents)
            {
                component.Dispose();
            }
            editorComponents.Clear();
            base.Dispose();
        }

        /// <summary>
        /// Apply the changes this editor has to the element. Returns true if changes have been made and the document needs refreshing.
        /// </summary>
        /// <param name="component"></param>
        /// <returns>True if changes are made.</returns>
        public bool applyChanges(RmlWysiwygComponent component)
        {
            if (hasChanges)
            {
                hasChanges = false;
                return elementStrategy.applyChanges(element, this, component);
            }
            return false;
        }

        /// <summary>
        /// Delete the element from the document if needed. Returns true if changes have been made and the document needs refreshing.
        /// </summary>
        /// <param name="component"></param>
        /// <returns>True if changes are made.</returns>
        public bool deleteIfNeeded(RmlWysiwygComponent component)
        {
            if (checkDeleteOnClose)
            {
                return elementStrategy.delete(element, this, component);
            }
            return false; //No changes made
        }

        /// <summary>
        /// Add an element editor to this editor, this class will take owenership of the object and dispose it for you.
        /// </summary>
        /// <param name="editorComponent"></param>
        public void addElementEditor(ElementEditorComponent editorComponent)
        {
            TabItem tab = tabs.addItem(editorComponent.Name);
            editorComponent.attachToParent(this, tab);
            editorComponents.Add(editorComponent);
        }

        public void cancelAndHide()
        {
            hasChanges = false;
            this.hide();
        }

        public String UndoRml { get; set; }

        internal void _changesMade()
        {
            hasChanges = true;
        }

        internal void _fireApplyChanges()
        {
            if (ChangesMade != null)
            {
                ChangesMade.Invoke(element);
            }
        }

        void delete_MouseButtonClick(Widget source, EventArgs e)
        {
            checkDeleteOnClose = false;
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

        void RmlElementEditor_Hiding(object sender, CancelEventArgs e)
        {
            if (hasChanges)
            {
                _fireApplyChanges();
            }
        }

        void close_MouseButtonClick(Widget source, EventArgs e)
        {
            hide();
        }
    }
}
