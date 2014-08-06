using Engine.Editing;
using Engine.ObjectManagement;
using Engine.Saving;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical
{
    public class PropDefinition : Saveable
    {
        private SimObjectDefinition simObject;
        private ShowPropTrackInfo trackInfo;

        public PropDefinition(SimObjectDefinition simObject)
        {
            PrettyName = Name = simObject.Name;
            this.simObject = simObject;
            trackInfo = new ShowPropTrackInfo();
        }

        [Editable]
        public String Name { get; set; }

        [Editable]
        public String PrettyName { get; set; }

        [Editable]
        public String BrowserPath { get; set; }

        [Editable]
        public long? PropLicenseId { get; set; }

        public bool RequireLicense
        {
            get
            {
                return PropLicenseId.HasValue;
            }

        }

        public SimObjectDefinition SimObject
        {
            get
            {
                return simObject;
            }
        }

        public ShowPropTrackInfo TrackInfo
        {
            get
            {
                return trackInfo;
            }
        }

        private EditInterface editInterface;
        public EditInterface EditInterface
        {
            get
            {
                if (editInterface == null)
                {
                    editInterface = ReflectedEditInterface.createEditInterface(this, "Prop Definition");
                    editInterface.addSubInterface(simObject.getEditInterface());
                    editInterface.addSubInterface(trackInfo.EditInterface);
                }
                return editInterface;
            }
        }

        protected PropDefinition(LoadInfo info)
        {
            Name = info.GetString("Name");
            PrettyName = info.GetString("PrettyName");
            BrowserPath = info.GetString("BrowserPath");
            simObject = info.GetValue<SimObjectDefinition>("SimObject");
            trackInfo = info.GetValue<ShowPropTrackInfo>("TrackInfo");
            if (info.hasValue("PropLicenseId"))
            {
                PropLicenseId = info.GetInt64("PropLicenseId");
            }
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Name", Name);
            info.AddValue("PrettyName", PrettyName);
            info.AddValue("BrowserPath", BrowserPath);
            info.AddValue("SimObject", simObject);
            info.AddValue("TrackInfo", trackInfo);
            if (PropLicenseId.HasValue)
            {
                info.AddValue("PropLicenseId", PropLicenseId.Value);
            }
        }
    }
}
