using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace Medical.GUI
{
    class AboutDialog : FixedSizeDialog
    {
        private Widget developerPanel;
        private Widget openSourcePanel;

        public AboutDialog(LicenseManager licenseManager)
            : base("Medical.GUI.AboutDialog.AboutDialog.layout")
        {
            developerPanel = window.findWidget("DeveloperPanel");
            openSourcePanel = window.findWidget("OpenSourcePanel");

            Button developerMoreButton = window.findWidget("DeveloperMoreButton") as Button;
            developerMoreButton.MouseButtonClick += new MyGUIEvent(developerMoreButton_MouseButtonClick);

            Button openSourceMoreButton = window.findWidget("OpenSourceMoreButton") as Button;
            openSourceMoreButton.MouseButtonClick += new MyGUIEvent(openSourceMoreButton_MouseButtonClick);

            Button closeButton = window.findWidget("CloseButton") as Button;
            closeButton.MouseButtonClick += new MyGUIEvent(closeButton_MouseButtonClick);

            TextBox versionText = window.findWidget("VersionText") as TextBox;
            versionText.Caption = String.Format("Version {0} {1}", Assembly.GetAssembly(typeof(AboutDialog)).GetName().Version, MedicalConfig.BuildName);

            TextBox anomalousMedicalText = window.findWidget("AnomalousMedicalLink") as TextBox;
            anomalousMedicalText.MouseButtonClick += new MyGUIEvent(anomalousMedicalText_MouseButtonClick);

            TextBox ogreText = window.findWidget("Ogre3DLink") as TextBox;
            ogreText.MouseButtonClick += new MyGUIEvent(ogreText_MouseButtonClick);

            TextBox myGUIText = window.findWidget("MyGUILink") as TextBox;
            myGUIText.MouseButtonClick += new MyGUIEvent(myGUIText_MouseButtonClick);

            TextBox bulletText = window.findWidget("BulletPhysicsLink") as TextBox;
            bulletText.MouseButtonClick += new MyGUIEvent(bulletText_MouseButtonClick);

            TextBox ZZiplibText = window.findWidget("ZZiplibLink") as TextBox;
            ZZiplibText.MouseButtonClick += new MyGUIEvent(ZZiplibText_MouseButtonClick);

            TextBox zlibText = window.findWidget("ZlibLink") as TextBox;
            zlibText.MouseButtonClick += new MyGUIEvent(zlibText_MouseButtonClick);

            TextBox freetypeText = window.findWidget("FreetypeLink") as TextBox;
            freetypeText.MouseButtonClick += new MyGUIEvent(freetypeText_MouseButtonClick);

            TextBox freeimageText = window.findWidget("FreeimageLink") as TextBox;
            freeimageText.MouseButtonClick += new MyGUIEvent(freeimageText_MouseButtonClick);

            TextBox openALText = window.findWidget("OpenALLink") as TextBox;
            openALText.MouseButtonClick += new MyGUIEvent(openALText_MouseButtonClick);

            TextBox oggVorbisText = window.findWidget("OggVorbisLink") as TextBox;
            oggVorbisText.MouseButtonClick += new MyGUIEvent(oggVorbisText_MouseButtonClick);

            TextBox wxWidgetsText = window.findWidget("wxWidgetsLink") as TextBox;
            wxWidgetsText.MouseButtonClick += new MyGUIEvent(wxWidgetsText_MouseButtonClick);

            TextBox monoText = window.findWidget("MonoLink") as TextBox;
            monoText.MouseButtonClick += new MyGUIEvent(monoText_MouseButtonClick);
        }

        void monoText_MouseButtonClick(Widget source, EventArgs e)
        {
            OtherProcessManager.openUrlInBrowser("http://www.mono-project.com");
        }

        void wxWidgetsText_MouseButtonClick(Widget source, EventArgs e)
        {
            OtherProcessManager.openUrlInBrowser("http://www.wxwidgets.org/");
        }

        void oggVorbisText_MouseButtonClick(Widget source, EventArgs e)
        {
            OtherProcessManager.openUrlInBrowser("http://www.vorbis.com/");
        }

        void openALText_MouseButtonClick(Widget source, EventArgs e)
        {
            OtherProcessManager.openUrlInBrowser("http://en.wikipedia.org/wiki/OpenAL");
        }

        void freeimageText_MouseButtonClick(Widget source, EventArgs e)
        {
            OtherProcessManager.openUrlInBrowser("http://freeimage.sourceforge.net/");
        }

        void freetypeText_MouseButtonClick(Widget source, EventArgs e)
        {
            OtherProcessManager.openUrlInBrowser("http://www.freetype.org/");
        }

        void zlibText_MouseButtonClick(Widget source, EventArgs e)
        {
            OtherProcessManager.openUrlInBrowser("http://www.zlib.net/");
        }

        void ZZiplibText_MouseButtonClick(Widget source, EventArgs e)
        {
            OtherProcessManager.openUrlInBrowser("http://zziplib.sourceforge.net/");
        }

        void bulletText_MouseButtonClick(Widget source, EventArgs e)
        {
            OtherProcessManager.openUrlInBrowser("http://www.bulletphysics.org/");
        }

        void myGUIText_MouseButtonClick(Widget source, EventArgs e)
        {
            OtherProcessManager.openUrlInBrowser("http://mygui.info/");
        }

        void ogreText_MouseButtonClick(Widget source, EventArgs e)
        {
            OtherProcessManager.openUrlInBrowser("http://www.ogre3d.org/");
        }

        void anomalousMedicalText_MouseButtonClick(Widget source, EventArgs e)
        {
            OtherProcessManager.openUrlInBrowser("http://www.anomalousmedical.com");
        }

        void closeButton_MouseButtonClick(Widget source, EventArgs e)
        {
            this.close();
        }

        void openSourceMoreButton_MouseButtonClick(Widget source, EventArgs e)
        {
            developerPanel.Visible = true;
            openSourcePanel.Visible = false;
        }

        void developerMoreButton_MouseButtonClick(Widget source, EventArgs e)
        {
            developerPanel.Visible = false;
            openSourcePanel.Visible = true;
        }
    }
}
