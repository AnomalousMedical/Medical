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

            StaticText versionText = window.findWidget("VersionText") as StaticText;
            versionText.Caption = "Version " + Assembly.GetAssembly(typeof(AboutDialog)).GetName().Version;

            StaticText anomalousMedicalText = window.findWidget("AnomalousMedicalLink") as StaticText;
            anomalousMedicalText.MouseButtonClick += new MyGUIEvent(anomalousMedicalText_MouseButtonClick);

            StaticText ogreText = window.findWidget("Ogre3DLink") as StaticText;
            ogreText.MouseButtonClick += new MyGUIEvent(ogreText_MouseButtonClick);

            StaticText myGUIText = window.findWidget("MyGUILink") as StaticText;
            myGUIText.MouseButtonClick += new MyGUIEvent(myGUIText_MouseButtonClick);

            StaticText bulletText = window.findWidget("BulletPhysicsLink") as StaticText;
            bulletText.MouseButtonClick += new MyGUIEvent(bulletText_MouseButtonClick);

            StaticText ZZiplibText = window.findWidget("ZZiplibLink") as StaticText;
            ZZiplibText.MouseButtonClick += new MyGUIEvent(ZZiplibText_MouseButtonClick);

            StaticText zlibText = window.findWidget("ZlibLink") as StaticText;
            zlibText.MouseButtonClick += new MyGUIEvent(zlibText_MouseButtonClick);

            StaticText freetypeText = window.findWidget("FreetypeLink") as StaticText;
            freetypeText.MouseButtonClick += new MyGUIEvent(freetypeText_MouseButtonClick);

            StaticText freeimageText = window.findWidget("FreeimageLink") as StaticText;
            freeimageText.MouseButtonClick += new MyGUIEvent(freeimageText_MouseButtonClick);

            StaticText openALText = window.findWidget("OpenALLink") as StaticText;
            openALText.MouseButtonClick += new MyGUIEvent(openALText_MouseButtonClick);

            StaticText oggVorbisText = window.findWidget("OggVorbisLink") as StaticText;
            oggVorbisText.MouseButtonClick += new MyGUIEvent(oggVorbisText_MouseButtonClick);

            StaticText wxWidgetsText = window.findWidget("wxWidgetsLink") as StaticText;
            wxWidgetsText.MouseButtonClick += new MyGUIEvent(wxWidgetsText_MouseButtonClick);

            StaticText monoText = window.findWidget("MonoLink") as StaticText;
            monoText.MouseButtonClick += new MyGUIEvent(monoText_MouseButtonClick);
        }

        void monoText_MouseButtonClick(Widget source, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.mono-project.com");
        }

        void wxWidgetsText_MouseButtonClick(Widget source, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.wxwidgets.org/");
        }

        void oggVorbisText_MouseButtonClick(Widget source, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.vorbis.com/");
        }

        void openALText_MouseButtonClick(Widget source, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://en.wikipedia.org/wiki/OpenAL");
        }

        void freeimageText_MouseButtonClick(Widget source, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://freeimage.sourceforge.net/");
        }

        void freetypeText_MouseButtonClick(Widget source, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.freetype.org/");
        }

        void zlibText_MouseButtonClick(Widget source, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.zlib.net/");
        }

        void ZZiplibText_MouseButtonClick(Widget source, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://zziplib.sourceforge.net/");
        }

        void oisText_MouseButtonClick(Widget source, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.wreckedgames.com/");
        }

        void bulletText_MouseButtonClick(Widget source, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.bulletphysics.org/");
        }

        void myGUIText_MouseButtonClick(Widget source, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://mygui.info/");
        }

        void ogreText_MouseButtonClick(Widget source, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.ogre3d.org/");
        }

        void anomalousMedicalText_MouseButtonClick(Widget source, EventArgs e)
        {
            Process.Start("http://www.anomalousmedical.com");
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
