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
        void createViews(String name, RunCommandsAction showCommand, AnomalousMvcContext context, Slide slide, bool allowPrevious, bool allowNext);

        SlideInstanceLayoutStrategy createLayoutStrategy();

        void addPanel(SlidePanel panel);

        void claimFiles(CleanupInfo info, ResourceProvider resourceProvider, Slide slide);

        SlideLayoutStrategy createDerivedStrategy(SlideLayoutStrategy oldStrategy, bool overwriteContent);

        SlidePanel getPanel(int index);

        IEnumerable<SlidePanel> Panels { get; }
    }
}
