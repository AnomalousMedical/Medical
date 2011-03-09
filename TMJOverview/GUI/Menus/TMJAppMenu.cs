using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MyGUIPlugin;
using Engine;
using System.IO;
using System.Reflection;
using Medical.GUI;
using System.Diagnostics;

namespace Medical.GUI
{
    class DopplerAppMenu : PopupContainer, AppMenu
    {
        private TMJOverviewGUIPlugin guiController;
        private StandaloneController standaloneController;

        public DopplerAppMenu(TMJOverviewGUIPlugin guiController, StandaloneController standaloneController)
            :base("Medical.GUI.Menus.TMJAppMenu.layout")
        {
            this.guiController = guiController;
            this.standaloneController = standaloneController;

            //Diagnose
            Button tmjOverview = widget.findWidget("TMJOverview") as Button;
            tmjOverview.MouseButtonClick += new MyGUIEvent(tmjOverview_MouseButtonClick);

            //Website
            Button website = widget.findWidget("Website") as Button;
            website.MouseButtonClick += new MyGUIEvent(website_MouseButtonClick);

            //Options
            Button optionsButton = widget.findWidget("Options") as Button;
            optionsButton.MouseButtonClick += new MyGUIEvent(optionsButton_MouseButtonClick);

            //Help
            Button helpButton = widget.findWidget("Help") as Button;
            helpButton.MouseButtonClick += new MyGUIEvent(helpButton_MouseButtonClick);

            //About
            Button aboutButton = widget.findWidget("About") as Button;
            aboutButton.MouseButtonClick += new MyGUIEvent(aboutButton_MouseButtonClick);

            //Update
            Button updateButton = widget.findWidget("Updates") as Button;
            updateButton.MouseButtonClick += new MyGUIEvent(updateButton_MouseButtonClick);

            //Quit
            Button quitButton = widget.findWidget("File/Quit") as Button;
            quitButton.MouseButtonClick += new MyGUIEvent(quitButton_MouseButtonClick);
        }

        void website_MouseButtonClick(Widget source, EventArgs e)
        {
            Process.Start("http://www.anomalousmedical.com");
            this.hide();
        }

        void tmjOverview_MouseButtonClick(Widget source, EventArgs e)
        {
            guiController.runTMJOverview();
            this.hide();
        }

        void quitButton_MouseButtonClick(Widget source, EventArgs e)
        {
            standaloneController.closeMainWindow();
            this.hide();
        }

        void optionsButton_MouseButtonClick(Widget source, EventArgs e)
        {
            //guiController.showOptions();
            this.hide();
        }

        void aboutButton_MouseButtonClick(Widget source, EventArgs e)
        {
            //guiController.showAboutDialog();
            this.hide();
        }

        void updateButton_MouseButtonClick(Widget source, EventArgs e)
        {
            UpdateManager.checkForUpdates(Assembly.GetAssembly(this.GetType()).GetName().Version, standaloneController.App.ProductID);
            this.hide();
        }

        void helpButton_MouseButtonClick(Widget source, EventArgs e)
        {
            standaloneController.openHelpTopic(0);
            this.hide();
        }
    }
}
