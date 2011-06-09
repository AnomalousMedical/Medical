using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using Engine;

namespace Medical.GUI
{
    class DialogEntry : MDIWindow
    {
        private Dialog dialog;
        private MDILayoutManager mdiManager;
        private int lastWidth = -1;
        private int lastHeight = -1;
        private IntVector2 captionMouseOffset;

        public DialogEntry(Dialog dialog, MDILayoutManager mdiManager)
        {
            this.dialog = dialog;
            this.mdiManager = mdiManager;
            CurrentlyVisible = dialog.Visible;
            dialog.Shown += new EventHandler(dialog_Shown);
            dialog.Closed += new EventHandler(dialog_Closed);
            dialog.ChangedCoord += new EventHandler(dialog_ChangedCoord);
            dialog.TempWindow.CaptionWidget.MouseButtonPressed += new MyGUIEvent(TempWindow_MouseButtonPressed);
            dialog.TempWindow.CaptionWidget.MouseButtonReleased += new MyGUIEvent(TempWindow_MouseButtonReleased);
            dialog.TempWindow.CaptionWidget.MouseDrag += new MyGUIEvent(TempWindow_MouseDrag);
        }

        void TempWindow_MouseDrag(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            IntVector2 mousePosition = new IntVector2(me.Position.x, me.Position.y);
            //dialog.Position = new Vector2(mousePosition.x - captionMouseOffset.x, mousePosition.y - captionMouseOffset.y);
            fireMouseDrag((MouseEventArgs)e);
        }

        void TempWindow_MouseButtonReleased(Widget source, EventArgs e)
        {
            fireMouseDragFinished((MouseEventArgs)e);
        }

        void TempWindow_MouseButtonPressed(Widget source, EventArgs e)
        {
            MouseEventArgs me = (MouseEventArgs)e;
            captionMouseOffset = new IntVector2(me.Position.x - dialog.TempWindow.CaptionWidget.AbsoluteLeft, me.Position.y - dialog.TempWindow.CaptionWidget.AbsoluteTop);
            layoutManager.ActiveWindow = this;
            fireMouseDragStarted(me);
        }

        void dialog_ChangedCoord(object sender, EventArgs e)
        {
            if (dialog.Width != lastWidth || dialog.Height != lastHeight)
            {
                lastWidth = dialog.Width;
                lastHeight = dialog.Height;
                invalidate();
            }
        }

        void dialog_Shown(object sender, EventArgs e)
        {
            dialog.IgnorePositionChanges = true;
            mdiManager.showWindow(this);
        }

        void dialog_Closed(object sender, EventArgs e)
        {
            dialog.IgnorePositionChanges = false;
            fireClosed();
        }

        public void tempClose()
        {
            CurrentlyVisible = dialog.Visible;
            dialog.Visible = false;
        }

        public void restoreState()
        {
            dialog.Visible = CurrentlyVisible;
        }

        public void serialize(ConfigFile file)
        {
            dialog.serialize(file);
        }

        public void deserialize(ConfigFile file)
        {
            dialog.deserialize(file);
        }

        public void ensureVisible()
        {
            dialog.ensureVisible();
        }

        public bool CurrentlyVisible { get; set; }

        protected override void activeStatusChanged(bool active)
        {

        }

        public override MDIWindow findWindowAtPosition(float mouseX, float mouseY)
        {
            return this;
        }

        public override void bringToFront()
        {

        }

        public override void setAlpha(float alpha)
        {

        }

        public override void layout()
        {
            dialog.Position = Location;
            dialog.Size = (IntSize2)WorkingSize;
        }

        public override Size2 DesiredSize
        {
            get
            {
                return new Size2(dialog.Width, dialog.Height);
            }
        }

        public override bool Visible
        {
            get
            {
                return dialog.Visible;
            }
            set
            {
                
            }
        }
    }
}
