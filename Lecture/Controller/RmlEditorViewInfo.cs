using Engine;
using Medical;
using Medical.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lecture
{
    class RmlEditorViewInfo
    {
        public RmlEditorViewInfo()
        {

        }

        public RmlEditorViewInfo(RawRmlWysiwygView view, SlidePanel slidePanel)
        {
            this.View = view;
            this.Panel = slidePanel;
        }

        public RawRmlWysiwygView View { get; set; }

        private RmlWysiwygComponent component;
        public RmlWysiwygComponent Component
        {
            get
            {
                return component;
            }
            set
            {
                if (component != null)
                {
                    component.Disposed -= component_Disposed;
                }
                component = value;
                if (component != null)
                {
                    component.Disposed += component_Disposed;
                }
            }
        }

        public SlidePanel Panel { get; set; }

        public String getCurrentComponentText()
        {
            if (Component != null)
            {
                return Component.CurrentRml;
            }
            return null;
        }

        internal bool commitText()
        {
            if (Component != null)
            {
                Component.aboutToSaveRml();
                bool changed = Component.ChangesMade;
                Component.ChangesMade = false;
                return changed;
            }
            return false;
        }

        internal void resizePanel(int size)
        {
            float ratio = (float)ScaleHelper.Scaled(Slideshow.BaseSlideScale) / Component.ViewHost.Container.RigidParentWorkingSize.Height;
            Panel.Size = (int)(size * ratio);
            Component.ViewHost.Container.invalidate();
        }

        internal void lostFocus()
        {
            if (Component != null)
            {
                component.cancelAndHideEditor();
                Component.clearPreviewElement();
            }
        }

        void component_Disposed()
        {
            component.Disposed -= component_Disposed;
            component = null;
        }
    }
}
