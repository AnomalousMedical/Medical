using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.GUI;
using Engine.Editing;
using Engine.Saving;

namespace Medical.Controller.AnomalousMvc
{
    public partial class AnomalousMvcContext : Saveable
    {
        private ControllerCollection controllers;
        private ViewCollection views;

        public AnomalousMvcContext()
        {
            controllers = new ControllerCollection();
            views = new ViewCollection();
        }

        public void changeLeftPanel(View view)
        {
            Core.changeLeftPanel(view);
        }

        public void stopTimelines()
        {
            Core.stopTimelines();
        }

        public void stopPlayingExample()
        {
            Core.stopPlayingExample();
        }

        public void playTimeline(String timelineName)
        {
            Core.playTimeline(timelineName, true);
        }

        public void playTimeline(String timelineName, bool allowPlaybackStop)
        {
            Core.playTimeline(timelineName, allowPlaybackStop);
        }

        public AnomalousMvcCore Core { get; set; }

        protected AnomalousMvcContext(LoadInfo info)
        {
            controllers = info.GetValue<ControllerCollection>("Controllers");
            views = info.GetValue<ViewCollection>("Views");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Controllers", controllers);
            info.AddValue("Views", views);
        }
    }

    public partial class AnomalousMvcContext
    {
        private EditInterface editInterface;

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = new EditInterface("MVC");
                editInterface.addSubInterface(controllers.getEditInterface());
                editInterface.addSubInterface(views.getEditInterface());
            }
            return editInterface;
        }
    }
}
