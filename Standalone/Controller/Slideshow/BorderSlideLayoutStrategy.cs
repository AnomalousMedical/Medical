using Engine;
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
        private Dictionary<ViewLocations, SlidePanel> panels = new Dictionary<ViewLocations, SlidePanel>();

        public BorderSlideLayoutStrategy()
        {

        }

        public void createViews(String name, RunCommandsAction showCommand, AnomalousMvcContext context, Slide slide, bool allowPrevious, bool allowNext)
        {
            bool addedButtons = false;
            foreach (SlidePanel panel in panels.Values)
            {
                MyGUIView view = panel.createView(slide, name);
                view.GetDesiredSizeOverride = layoutView;
                if (!addedButtons)
                {
                    addedButtons = true;
                    if (allowPrevious)
                    {
                        view.Buttons.add(new ButtonDefinition("Previous", "NavigationBug/Previous"));
                    }
                    if (allowNext)
                    {
                        view.Buttons.add(new ButtonDefinition("Next", "NavigationBug/Next"));
                    }
                    view.Buttons.add(new CloseButtonDefinition("Close", "Common/Close"));
                }
                showCommand.addCommand(new ShowViewCommand(view.Name));
                context.Views.add(view);
            }
        }

        public void addPanel(SlidePanel panel)
        {
            panels.Add(panel.ViewLocation, panel);
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
                SlidePanel existingPanel = oldStrategy.getPanel((int)panel.ViewLocation);
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

        public SlidePanel getPanel(int index)
        {
            SlidePanel ret = null;
            if (panels.TryGetValue((ViewLocations)index, out ret))
            {
                return ret;
            }
            return null;
        }

        private IntSize2 layoutView(LayoutContainer layoutContainer, Widget widget, MyGUIView view)
        {
            SlidePanel panel = panels[view.ViewLocation];
            switch (view.ViewLocation)
            {
                case ViewLocations.Left:
                    return new IntSize2(panel.Size, widget.Height);
                case ViewLocations.Right:
                    return new IntSize2(panel.Size, widget.Height);
                case ViewLocations.Top:
                    return new IntSize2(widget.Width, panel.Size);
                case ViewLocations.Bottom:
                    return new IntSize2(widget.Width, panel.Size);
                default:
                    return new IntSize2(panel.Size, panel.Size);
            }
        }

        protected BorderSlideLayoutStrategy(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info);
            info.RebuildDictionary("Panel", panels);
        }

        public void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info);
            info.ExtractDictionary("Panel", panels);
        }
    }
}
