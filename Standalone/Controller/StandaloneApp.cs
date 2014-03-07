﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medical.Controller
{
    public abstract class StandaloneApp : App
    {
        public StandaloneApp()
        {
            
        }

        public abstract String WindowTitle { get; }

        public abstract String PrimaryArchive { get; }

        public abstract String DefaultScene { get; }

        public LicenseManager LicenseManager { get; protected set; }
    }
}
