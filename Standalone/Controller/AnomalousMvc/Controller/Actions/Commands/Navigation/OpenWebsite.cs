using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Editor;
using Engine.Saving;
using Logging;
using Engine.Editing;

namespace Medical.Controller.AnomalousMvc
{
    /// <summary>
    /// A class to open a website, right now this is limited to urls on the WebsiteHostUrl website.
    /// </summary>
    class OpenWebsite : ActionCommand
    {
        public OpenWebsite()
        {
            
        }

        public override void execute(AnomalousMvcContext context)
        {
            OtherProcessManager.openUrlInBrowser(String.Format("{0}/{1}", MedicalConfig.WebsiteHostUrl, InSiteUrl));
        }

        public override string Type
        {
            get
            {
                return "Open Web Page";
            }
        }

        public override string Icon
        {
            get
            {
                return CommonResources.NoIcon;
            }
        }

        [Editable]
        public String InSiteUrl { get; set; }

        protected OpenWebsite(LoadInfo info)
            : base(info)
        {

        }
    }
}
