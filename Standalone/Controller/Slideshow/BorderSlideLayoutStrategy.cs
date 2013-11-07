using Engine.Attributes;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using Medical.GUI.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    class BorderSlideLayoutStrategy : SlideLayoutStrategy
    {
        [DoNotSave]
        private List<SlidePanel> panels = new List<SlidePanel>();

        public BorderSlideLayoutStrategy()
        {

        }

        public void createViews(String name, RunCommandsAction showCommand, AnomalousMvcContext context, Slide slide, bool allowPrevious, bool allowNext)
        {
            bool addedButtons = false;
            foreach (SlidePanel panel in panels)
            {
                MyGUIView view = panel.createView(slide, name);
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
            panels.Add(panel);
        }

        public void claimFiles(CleanupInfo info, ResourceProvider resourceProvider, Slide slide)
        {
            foreach (RmlSlidePanel panel in panels)
            {
                panel.claimFiles(info, resourceProvider, slide);
            }
        }

        public SlideLayoutStrategy createDerivedStrategy(SlideLayoutStrategy oldStrategy, bool overwriteContent)
        {
            BorderSlideLayoutStrategy borderSlides = new BorderSlideLayoutStrategy();
            int index = 0;
            foreach (SlidePanel panel in panels)
            {
                SlidePanel existingPanel = oldStrategy.getPanel(index++);
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
                return panels;
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
            if (index < panels.Count)
            {
                return panels[index];
            }
            return null;
        }

        protected BorderSlideLayoutStrategy(LoadInfo info)
        {
            ReflectedSaver.RestoreObject(this, info);
            info.RebuildList("Panel", panels);
        }

        public void getInfo(SaveInfo info)
        {
            ReflectedSaver.SaveObject(this, info);
            info.ExtractList("Panel", panels);
        }
    }
}
