using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Saving;

namespace Medical
{
    class StartExamTimelineTask : Task, Saveable
    {
        public StartExamTimelineTask(String uniqueName, String name, String iconName, String category)
            :base(uniqueName, name, iconName, category)
        {

        }

        public override void clicked()
        {
            
        }

        public override bool Active
        {
            get
            {
                return false;
            }
        }

        protected StartExamTimelineTask(LoadInfo info)
            : base(info.GetString("uniqueName"), info.GetString("name"), info.GetString("iconName"), info.GetString("category"))
        {

        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("uniqueName", UniqueName);
            info.AddValue("name", Name);
            info.AddValue("iconName", IconName);
            info.AddValue("category", Category);
        }
    }
}
