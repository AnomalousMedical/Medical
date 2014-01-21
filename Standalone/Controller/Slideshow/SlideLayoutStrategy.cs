using Engine.Attributes;
using Engine.Saving;
using Medical.Controller.AnomalousMvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical
{
    public interface SlideLayoutStrategy : Saveable
    {
        void createViews(String name, RunCommandsAction showCommand, AnomalousMvcContext context, SlideDisplayManager displayManager, Slide slide);

        SlideInstanceLayoutStrategy createLayoutStrategy(SlideDisplayManager displayManager);

        void addPanel(SlidePanel panel);

        void claimFiles(CleanupInfo info, ResourceProvider resourceProvider, Slide slide);

        SlideLayoutStrategy createDerivedStrategy(SlideLayoutStrategy oldStrategy, bool overwriteContent);

        SlidePanel getPanel(LayoutElementName name);

        IEnumerable<SlidePanel> Panels { get; }
    }
}
