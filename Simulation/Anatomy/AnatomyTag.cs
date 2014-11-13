using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Editing;
using Engine.Saving;

namespace Medical
{
    class AnatomyTag : Saveable
    {
        public AnatomyTag()
        {

        }

        [Editable]
        public String Tag { get; set; }

        #region Saveable Members

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Tag", Tag);
        }

        protected AnatomyTag(LoadInfo info)
        {
            Tag = info.GetString("Tag");
        }

        #endregion
    }
}
