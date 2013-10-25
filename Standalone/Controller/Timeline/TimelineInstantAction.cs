using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;
using Engine.Editing;

namespace Medical
{
    public abstract class TimelineInstantAction : Saveable
    {
        private Timeline timeline;
        private EditInterface editInterface;

        protected TimelineInstantAction()
        {

        }

        internal void _setTimeline(Timeline timeline)
        {
            this.timeline = timeline;
        }

        public abstract void doAction();

        public abstract void dumpToLog();

        public abstract void findFileReference(TimelineStaticInfo info);

        public abstract void cleanup(CleanupInfo cleanupInfo);

        public EditInterface getEditInterface()
        {
            if (editInterface == null)
            {
                editInterface = ReflectedEditInterface.createEditInterface(this, ReflectedEditInterface.DefaultScanner, GetType().Name, null);
                customizeEditInterface(editInterface);
            }
            return editInterface;
        }

        protected virtual void customizeEditInterface(EditInterface editInterface)
        {

        }

        public TimelineController TimelineController
        {
            get
            {
                return timeline.TimelineController;
            }
        }

        public Timeline Timeline
        {
            get
            {
                return timeline;
            }
        }

        #region Saveable Members

        protected TimelineInstantAction(LoadInfo info)
        {

        }

        public virtual void getInfo(SaveInfo info)
        {
            
        }

        #endregion
    }
}
