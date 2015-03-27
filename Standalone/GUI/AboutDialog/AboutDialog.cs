﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Medical.Controller;
using System.Reflection;
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

            TextBox oggVorbisText = window.findWidget("OggVorbisLink") as TextBox;
            oggVorbisText.MouseButtonClick += new MyGUIEvent(oggVorbisText_MouseButtonClick);

            TextBox monoText = window.findWidget("MonoLink") as TextBox;
            monoText.MouseButtonClick += new MyGUIEvent(monoText_MouseButtonClick);

            TextBox libRocketLink = window.findWidget("LibRocketLink") as TextBox;
            libRocketLink.MouseButtonClick += libRocketLink_MouseButtonClick;

            TextBox openAlSoftLink = window.findWidget("OpenALSoftLink") as TextBox;
            openAlSoftLink.MouseButtonClick += openAlSoftLink_MouseButtonClick;

            TextBox luceneNetLink = window.findWidget("LuceneNetLink") as TextBox;
            luceneNetLink.MouseButtonClick += luceneNetLink_MouseButtonClick;

            TextBox bepuPhysicsLink = window.findWidget("BEPUPhysicsLink") as TextBox;
            bepuPhysicsLink.MouseButtonClick += bepuPhysicsLink_MouseButtonClick;

            TextBox copyrightText = window.findWidget("CopyrightText") as TextBox;
            copyrightText.Caption = String.Format("Copyright 2009-{0} Anomalous Medical, LLC", DateTime.Now.Year);

        }

        void monoText_MouseButtonClick(Widget source, EventArgs e)
        {
            openUrl("http://www.mono-project.com");
        }

        void oggVorbisText_MouseButtonClick(Widget source, EventArgs e)
        {
            openUrl("http://www.vorbis.com/");
        }

        void openALText_MouseButtonClick(Widget source, EventArgs e)
        {
            openUrl("http://en.wikipedia.org/wiki/OpenAL");
        }

        void freeimageText_MouseButtonClick(Widget source, EventArgs e)
        {
            openUrl("http://freeimage.sourceforge.net/");
        }

        void freetypeText_MouseButtonClick(Widget source, EventArgs e)
        {
            openUrl("http://www.freetype.org/");
        }

        void zlibText_MouseButtonClick(Widget source, EventArgs e)
        {
            openUrl("http://www.zlib.net/");
        }

        void ZZiplibText_MouseButtonClick(Widget source, EventArgs e)
        {
            openUrl("http://zziplib.sourceforge.net/");
        }

        void bulletText_MouseButtonClick(Widget source, EventArgs e)
        {
            openUrl("http://www.bulletphysics.org/");
        }

        void myGUIText_MouseButtonClick(Widget source, EventArgs e)
        {
            openUrl("http://mygui.info/");
        }

        void ogreText_MouseButtonClick(Widget source, EventArgs e)
        {
            openUrl("http://www.ogre3d.org/");
        }

        void anomalousMedicalText_MouseButtonClick(Widget source, EventArgs e)
        {
            openUrl("http://www.anomalousmedical.com");
        }

        void libRocketLink_MouseButtonClick(Widget source, EventArgs e)
        {
            openUrl("http://librocket.com/");
        }

        void openAlSoftLink_MouseButtonClick(Widget source, EventArgs e)
        {
            openUrl("http://kcat.strangesoft.net/openal.html");
        }

        void luceneNetLink_MouseButtonClick(Widget source, EventArgs e)
        {
            openUrl("http://lucenenet.apache.org/");
        }

        void bepuPhysicsLink_MouseButtonClick(Widget source, EventArgs e)
        {
            openUrl("http://bepuphysics.codeplex.com/");
        }

        private void openUrl(String url)
        {
            if (PlatformConfig.UnrestrictedEnvironment)
            {
                OtherProcessManager.openUrlInBrowser(url);
            }
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
