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

        public PropDefinition(SimObjectDefinition simObject)
        {
            this.simObject = simObject;
        }

        [Editable]
        public String Name { get; set; }

        public SimObjectDefinition SimObject
        {
            get
            {
                return simObject;
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
                }
                return editInterface;
            }
        }

        protected PropDefinition(LoadInfo info)
        {
            Name = info.GetString("Name");
            simObject = info.GetValue<SimObjectDefinition>("SimObject");
        }

        public void getInfo(SaveInfo info)
        {
            info.AddValue("Name", Name);
            info.AddValue("SimObject", simObject);
        }
    }
}
