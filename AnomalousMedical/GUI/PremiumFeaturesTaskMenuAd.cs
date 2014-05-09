using Engine;
using MyGUIPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Medical.GUI
{
    class PremiumFeaturesTaskMenuAd : TaskMenuAdProvider, IDisposable
    {
        TaskMenu taskMenu;
        ImageBox adImage;

        private static readonly int AdWidth = ScaleHelper.Scaled(230);
        private static readonly int AdHeight = ScaleHelper.Scaled(460);

        public PremiumFeaturesTaskMenuAd(TaskMenu taskMenu)
        {
            this.taskMenu = taskMenu;
            taskMenu.AdProvider = this;
            taskMenu.Showing += taskMenu_Showing;
        }

        public void Dispose()
        {
            taskMenu.Showing -= taskMenu_Showing;
            if (adImage != null)
            {
                fireAdDestroyed();
                Gui.Instance.destroyWidget(adImage);
            }
            if (taskMenu.AdProvider == this)
            {
                taskMenu.AdProvider = null;
            }
        }

        void taskMenu_Showing(object sender, EventArgs e)
        {
            if (adImage == null)
            {
                adImage = (ImageBox)ParentWidget.createWidgetT("ImageBox", "ImageBox", 2, taskMenu.AdTop, AdWidth, AdHeight, Align.Left | Align.Top, "");
                adImage.setItemResource("AnomalousMedical/PremiumAd");
                adImage.MouseButtonClick += adImage_MouseButtonClick;
                Right = adImage.Right;
                fireAdCreated();
                taskMenu.Showing -= taskMenu_Showing;
            }
        }

        void adImage_MouseButtonClick(Widget source, EventArgs e)
        {
            OtherProcessManager.openUrlInBrowser(MedicalConfig.DefaultAdUrl);
        }
    }
}
