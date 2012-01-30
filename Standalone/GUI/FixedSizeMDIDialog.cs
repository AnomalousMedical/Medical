using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine;
using Medical.Controller;

namespace Medical.GUI
{
    public class FixedSizeMDIDialog : MDIDialog
    {
        public FixedSizeMDIDialog(String layoutFile)
            : base(layoutFile)
        {

        }

        public override void deserialize(ConfigFile configFile)
        {
            ConfigSection section = configFile.createOrRetrieveConfigSection(PersistName);
            loadDockProperties(section);
            String location = section.getValue("Location", DesiredLocation.ToString());
            Rect desiredLocation = new Rect();
            desiredLocation.fromString(location);

            if (desiredLocation.Left < 0)
            {
                desiredLocation.Left = 0;
            }
            if (desiredLocation.Top < 0)
            {
                desiredLocation.Top = 0;
            }

            DesiredLocation = desiredLocation;

            window.setCoord((int)desiredLocation.Left, (int)desiredLocation.Top, window.Width, window.Height);
        }
    }
}
