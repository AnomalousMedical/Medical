using Engine;
using libRocketPlugin;
using Medical.Controller;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace Medical.GUI.RmlWysiwyg.ElementEditorComponents
{
    public class ElementTextEditor : ElementEditorComponent
    {
        private const int UPDATE_DELAY = 500;

        private EditBox text;
        private Timer keyTimer;

        public ElementTextEditor(String startingText)
            : base("Medical.GUI.Editor.RmlWysiwyg.ElementEditorComponents.ElementTextEditor.layout", "Text")
        {
            text = (EditBox)widget;
            InputManager.Instance.setKeyFocusWidget(text);
            Text = startingText;
            keyTimer = new Timer(UPDATE_DELAY);
            keyTimer.SynchronizingObject = new ThreadManagerSynchronizeInvoke();
            keyTimer.AutoReset = false;
            keyTimer.Elapsed += keyTimer_Elapsed;
        }

        public override void Dispose()
        {
            keyTimer.Dispose();
        }

        public override void attachToParent(RmlElementEditor parentEditor, Widget parent)
        {
            base.attachToParent(parentEditor, parent);
            text.KeyButtonReleased += text_KeyButtonReleased;
            InputManager.Instance.setKeyFocusWidget(text);
        }

        public String Text
        {
            get
            {
                return text.OnlyText;
            }
            private set
            {
                text.OnlyText = value.Replace("\r", "");
            }
        }

        void text_KeyButtonReleased(Widget source, EventArgs e)
        {
            this.fireChangesMade();
            keyTimer.Stop();
            keyTimer.Start();
        }

        void keyTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            fireApplyChanges();
        }
    }
}
