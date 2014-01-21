﻿using Engine;
using Engine.Attributes;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class BorderSlideLayoutStrategy : SlideLayoutStrategy
    {
        [DoNotSave]
        private Dictionary<LayoutElementName, SlidePanel> panels = new Dictionary<LayoutElementName, SlidePanel>();

        public BorderSlideLayoutStrategy()
        {

        }

        public void createViews(String name, RunCommandsAction showCommand, AnomalousMvcContext context, Slide slide)
        {
            SlideInstanceLayoutStrategy instanceStrategy = createLayoutStrategy();
            foreach (SlidePanel panel in panels.Values)
            {
                MyGUIView view = panel.createView(slide, name);
                instanceStrategy.addView(view);
                showCommand.addCommand(new ShowViewCommand(view.Name));
                context.Views.add(view);
            }
        }

        public SlideInstanceLayoutStrategy createLayoutStrategy()
        {
            return new InstanceStrategy(this);
        }

        public void addPanel(SlidePanel panel)
        {
            panels.Add(panel.ElementName, panel);
        }

        public void claimFiles(CleanupInfo info, ResourceProvider resourceProvider, Slide slide)
        {
            foreach (RmlSlidePanel panel in panels.Values)
            {
                panel.claimFiles(info, resourceProvider, slide);
            }
        }

        public SlideLayoutStrategy createDerivedStrategy(SlideLayoutStrategy oldStrategy, bool overwriteContent)
        {
            BorderSlideLayoutStrategy borderSlides = new BorderSlideLayoutStrategy();
            foreach (SlidePanel panel in panels.Values)
            {
                SlidePanel existingPanel = oldStrategy.getPanel(panel.ElementName);
                if (existingPanel != null)
                {
                    existingPanel = existingPanel.clone();
                    panel.applyToExisting(existingPanel, overwriteContent);
                }
                else
                {
                    existingPanel = panel.clone();
                }
                borderSlides.addPanel(existingPanel);
            }
            return borderSlides;
        }

        public IEnumerable<SlidePanel> Panels
        {
            get
            {
                return panels.Values;
            }
        }

        public int PanelCount
        {
            get
            {
                return panels.Count;
            }
        }

        public SlidePanel getPanel(LayoutElementName name)
        {
            SlidePanel ret = null;
            if (panels.TryGetValue(name, out ret))
            {
                return ret;
            }
            return null;
        }

        private const int CurrentVersion = 1;

        protected BorderSlideLayoutStrategy(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info);
            if (info.Version < CurrentVersion)
            {
                if (info.Version < 1)
                {
                    Dictionary<ViewLocations, SlidePanel> legacyPanels = new Dictionary<ViewLocations, SlidePanel>();
                    info.RebuildDictionary("Panel", legacyPanels);
                    foreach (var panel in legacyPanels.Values)
                    {
                        addPanel(panel);
                    }
                }
            }
            else
            {
                info.RebuildDictionary("Panel", panels);
            }
        }

        public void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info);
            info.ExtractDictionary("Panel", panels);
            info.Version = CurrentVersion;
        }

        class InstanceStrategy : SlideInstanceLayoutStrategy
        {
            private Dictionary<LayoutElementName, MyGUIViewHost> viewHosts = new Dictionary<LayoutElementName, MyGUIViewHost>();
            private BorderSlideLayoutStrategy masterStrategy;
            private int lastWorkingParentHeight = int.MinValue;

            public InstanceStrategy(BorderSlideLayoutStrategy masterStrategy)
            {
                this.masterStrategy = masterStrategy;
            }

            public void addView(MyGUIView view)
            {
                view.ViewHostCreated += view_ViewHostCreated;
                view.GetDesiredSizeOverride = this.layoutView;
            }

            public IntSize2 layoutView(LayoutContainer layoutContainer, Widget widget, MyGUIView view)
            {
                float ratio = layoutContainer.RigidParentWorkingSize.Height / (float)Slideshow.BaseSlideScale;
                SlidePanel panel = masterStrategy.panels[view.ElementName];
                int size = (int)(ScaleHelper.Scaled(panel.Size) * ratio);

                switch (view.ElementName.LocationHint)
                {
                    case ViewLocations.Left:
                        return new IntSize2(size, widget.Height);
                    case ViewLocations.Right:
                        return new IntSize2(size, widget.Height);
                    case ViewLocations.Top:
                        return new IntSize2(widget.Width, size);
                    case ViewLocations.Bottom:
                        return new IntSize2(widget.Width, size);
                    default:
                        return new IntSize2(size, size);
                }
            }

            void view_ViewHostCreated(MyGUIViewHost view)
            {
                viewHosts.Add(view.View.ElementName, view);
                view.ViewClosing += view_ViewClosing;
                view.ViewResized += view_ViewResized;
                lastWorkingParentHeight = int.MinValue;
            }

            void view_ViewResized(ViewHost view)
            {
                IntSize2 rigidParentSize = view.Container.RigidParentWorkingSize;
                if (rigidParentSize.Height != lastWorkingParentHeight)
                {
                    float ratio = rigidParentSize.Height / (float)Slideshow.BaseSlideScale;
                    SlidePanel panel = masterStrategy.panels[view.View.ElementName];
                    int size = (int)(ScaleHelper.Scaled(panel.Size) * ratio);
                    if (viewHosts.ContainsKey(view.View.ElementName))
                    {
                        viewHosts[view.View.ElementName].changeScale(ratio);
                    }
                }
            }

            void view_ViewClosing(ViewHost view)
            {
                viewHosts.Remove(view.View.ElementName);
            }
        }
    }
}
