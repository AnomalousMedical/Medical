using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Engine.Saving;

namespace Medical
{
    public sealed class ShowPropSubActionPrototype : Saveable
    {
        private Type type;

        public ShowPropSubActionPrototype(Type type, String trackName)
        {
            this.type = type;
            this.TrackName = trackName;
        }

        public ShowPropSubAction createSubAction()
        {
            return (ShowPropSubAction)Activator.CreateInstance(type);
        }

        public String TrackName { get; private set; }

        private ShowPropSubActionPrototype(LoadInfo info)
        {
            TrackName = info.GetString("TrackName");
            type = DefaultTypeFinder.FindType(info.GetString("TypeName"));
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("TrackName", TrackName);
            info.AddValue("TypeName", DefaultTypeFinder.CreateShortTypeString(type));
        }
    }
}
