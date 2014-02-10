using Engine;
using Medical;
using Medical.Controller.AnomalousMvc;
using Medical.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lecture
{
    class RmlEditorViewInfo
    {
        /// <summary>
        /// This event will fire when the view for an editor has resized. Subscribing to this instead of
        /// the wrapped component events will allow the event subscription to work even if the component is
        /// currently not available.
        /// </summary>
        public event Action<ViewHost> ViewResized;

        private TextCachedResource cachedResource;

        public RmlEditorViewInfo(RmlWysiwygView view, SlidePanel slidePanel, EditorResourceProvider resourceProvider)
        {
            this.View = view;
            this.Panel = slidePanel;
            cachedResource = new ResourceProviderTextCachedResource(view.RmlFile, Encoding.UTF8, "", resourceProvider);
        }

        public RmlWysiwygView View { get; set; }

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
                    component.ViewHost.ViewResized -= ViewHost_ViewResized;
                }
                component = value;
                if (component != null)
                {
                    component.Disposed += component_Disposed;
                    component.ViewHost.ViewResized += ViewHost_ViewResized;
                }
            }
        }

        public SlidePanel Panel { get; set; }

        public TextCachedResource CachedResource
        {
            get
            {
                return cachedResource;
            }
        }

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
            float ratio = Component.getScale();
            if (ratio == 0.0f)
            {
                ratio = 1f;
            }
            else
            {
                ratio = 1f / ratio;
            }
            Panel.Size = ScaleHelper.Unscaled((int)(size * ratio));
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

        void ViewHost_ViewResized(ViewHost obj)
        {
            if (ViewResized != null)
            {
                ViewResized.Invoke(obj);
            }
        }
    }
}
