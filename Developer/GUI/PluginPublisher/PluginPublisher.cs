using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Medical.Controller;
using Medical.GUI;
using MyGUIPlugin;
using Medical;

namespace Developer.GUI
{
    class PluginPublisher : MDIDialog
    {
        private const String PLUGIN_WILDCARD = "Data Driven Plugin (*.ddp)|*.ddp;";
        private const String SIGNATURE_WILDCARD = "Signature File (*.xml)|*.xml;";

        private Edit pluginFileEdit;
        private Edit signatureFileEdit;
        private Edit outDirEdit;

        public PluginPublisher()
            :base("Developer.GUI.PluginPublisher.PluginPublisher.layout")
        {
            pluginFileEdit = (Edit)window.findWidget("PluginFileEdit");
            signatureFileEdit = (Edit)window.findWidget("SignatureFileEdit");
            outDirEdit = (Edit)window.findWidget("OutDirEdit");

            Button pluginFileBrowser = (Button)window.findWidget("PluginFileBrowser");
            pluginFileBrowser.MouseButtonClick += new MyGUIEvent(pluginFileBrowser_MouseButtonClick);

            Button signatureFileBrowser = (Button)window.findWidget("SignatureFileBrowser");
            signatureFileBrowser.MouseButtonClick += new MyGUIEvent(signatureFileBrowser_MouseButtonClick);

            Button outDirBrowser = (Button)window.findWidget("OutDirBrowser");
            outDirBrowser.MouseButtonClick += new MyGUIEvent(outDirBrowser_MouseButtonClick);
        }

        void outDirBrowser_MouseButtonClick(Widget source, EventArgs e)
        {
            using (DirDialog dirDialog = new DirDialog(MainWindow.Instance, "Select an out directory", MedicalConfig.UserDocRoot))
            {
                if (dirDialog.showModal() == NativeDialogResult.OK)
                {
                    outDirEdit.Caption = dirDialog.Path;
                }
            }
        }

        void signatureFileBrowser_MouseButtonClick(Widget source, EventArgs e)
        {
            using (FileOpenDialog openDialog = new FileOpenDialog(MainWindow.Instance, "Select a signature file", MedicalConfig.UserDocRoot, "", SIGNATURE_WILDCARD, false))
            {
                if (openDialog.showModal() == NativeDialogResult.OK)
                {
                    signatureFileEdit.Caption = openDialog.Path;
                }
            }
        }

        void pluginFileBrowser_MouseButtonClick(Widget source, EventArgs e)
        {
            using (FileOpenDialog openDialog = new FileOpenDialog(MainWindow.Instance, "Select a plugin file", MedicalConfig.UserDocRoot, "", PLUGIN_WILDCARD, false))
            {
                if (openDialog.showModal() == NativeDialogResult.OK)
                {
                    pluginFileEdit.Caption = openDialog.Path;
                }
            }
        }
    }
}
