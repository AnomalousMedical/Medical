using libRocketPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    class BuyScreenEventController : RocketEventController
    {
        private BuyScreen buyScreen;

        public BuyScreenEventController(BuyScreen buyScreen)
        {
            this.buyScreen = buyScreen;
        }

        public EventListener createEventListener(string name)
        {
            switch(name)
            {
                case "close":
                    return new BuyScreenEventListener((evt) =>
                    {
                        buyScreen.hide();
                    });
                case "visitPage":
                    return new BuyScreenEventListener(evt =>
                    {
                        Variant url = evt.TargetElement.GetAttribute("url");
                        if (url != null)
                        {
                            OtherProcessManager.openUrlInBrowser(String.Format("{0}/{1}", MedicalConfig.WebsiteHostUrl, url.StringValue));
                        }
                    });
                default:
                    return new BuyScreenEventListener();
            }
        }
    }
}
